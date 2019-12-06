using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using JetBrains.Annotations;

using MathUtilities;
using RenderDataStructures.Shapes;
using RenderDataStructures.Cameras;
using RenderDataStructures.Materials;
using Color = MathUtilities.Color;
using SystemFonts = SixLabors.Fonts.SystemFonts;

namespace RenderHandler
{
    public static class ImageLock
    {
        public static object Lock { get; set; } = new object();
    }

    public class ThreadSafeImage
    {
        private Bitmap m_image;

        public Bitmap Image
        {
            get
            {
                lock (ImageLock.Lock)
                {
                    return m_image;
                }
            }
            set
            {
                lock (ImageLock.Lock)
                {
                    if (m_image == value) return;
                    m_image = value;
                }
            }
        }
        public bool IsLocked { get; set; }
    }
    public class Renderer : INotifyPropertyChanged
    {
        public ThreadSafeImage ImageBuffer { get; set; }

        private bool m_isRendering;

        public bool IsRendering
        {
            get => m_isRendering;
            set
            {
                if (m_isRendering == value) return;
                m_isRendering = value;
                OnPropertyChanged(nameof(IsRendering));
            }
        }

        private int m_totalPixels;
        public int TotalPixels
        {
            get => m_totalPixels;
            set
            {
                if (m_totalPixels == value) return;
                m_totalPixels = value;
                OnPropertyChanged(nameof(TotalPixels));
            }
        }

        private int m_processedPixels;
        public int ProcessedPixels
        {
            get => m_processedPixels;
            set
            {
                if (m_processedPixels == value) return;
                m_processedPixels = value;
                OnPropertyChanged(nameof(ProcessedPixels));
            }
        }

        public Color GetColor(ref Ray p_ray, IHitTarget p_world, int p_totalDepth)
        {
            var hitRecord = new HitRecord();

            if (!p_world.WasHit(p_ray, 0.001d, double.MaxValue, ref hitRecord)) return new Color(0, 0, 0);
            var scatteredRay = new Ray(new Vec3(0, 0, 0), new Vec3(0, 0, 0));
            var attenuation = new Color(0, 0, 0);
            var emitted = hitRecord.Material.GetEmitted(hitRecord.U, hitRecord.V, hitRecord.P);

            if (p_ray.Depth < p_totalDepth && hitRecord.Material.ScatterRay(ref p_ray, ref hitRecord, ref attenuation, ref scatteredRay))
            {
                return emitted + attenuation * GetColor(ref scatteredRay, p_world, p_totalDepth);
            }

            return emitted;

        }

        public void DoRender(RenderParameters p_renderParameters, out TimeSpan p_renderTime)
        {
            using var bufferRefreshTimer = new System.Timers.Timer { Interval = 250, AutoReset = true };

            if (p_renderParameters.RealTimeUpdate)
            {
                // Hook up the Elapsed event for the timer. 
                bufferRefreshTimer.Elapsed += BufferRefreshEvent;

                // Start the timer
                bufferRefreshTimer.Enabled = true;
            }

            IsRendering = true;

            ImageBuffer = new ThreadSafeImage()
            {
                Image = new Bitmap(p_renderParameters.XResolution, p_renderParameters.YResolution),
                IsLocked = false
            };
            
            using var image = new Image<Rgba32>(p_renderParameters.XResolution, p_renderParameters.YResolution);

            var stopWatch = Stopwatch.StartNew();

            var newScene = SceneGenerator.GenerateCornellBoxBvhScene(p_renderParameters);

            var renderChunks = new List<RenderChunk>();

            var availableThreads = Environment.ProcessorCount;

            if (p_renderParameters.YResolution < availableThreads || !p_renderParameters.Parallel)
            {
                var renderChunk = new RenderChunk(0, p_renderParameters.YResolution - 1);
                renderChunks.Add(renderChunk);
            }
            else
            {
                var chunkSize = p_renderParameters.YResolution / availableThreads;

                for (var i = 0; i < availableThreads; ++i)
                {
                    if (i == availableThreads - 1)
                    {
                        var renderChunk = new RenderChunk(i * chunkSize, p_renderParameters.YResolution - 1);
                        renderChunks.Add(renderChunk);
                    }
                    else
                    {
                        var renderChunk = new RenderChunk(i * chunkSize, i * chunkSize + chunkSize - 1);
                        renderChunks.Add(renderChunk);
                    }
                }
            }

            var renderTasks = new List<Task>();

            foreach (var renderChunk in renderChunks)
            {
                var renderTask = Task.Factory.StartNew(() => RenderSomeChunk(image, newScene.Camera, newScene.World, renderChunk.StartRow, renderChunk.EndRow, p_renderParameters));
                renderTasks.Add(renderTask);
            }

            Task.WaitAll(renderTasks.ToArray());

            stopWatch.Stop();

            // Optionally set encoder values by hand. - Comment by Matt Heimlich on 11/8/2019 @ 18:05:42
            //Configuration.Default.ImageFormatsManager.SetEncoder(PngFormat.Instance, new PngEncoder()
            //{
            //    BitDepth = PngBitDepth.Bit8,
            //    ColorType = PngColorType.Rgb,
            //    CompressionLevel = 6,
            //    FilterMethod = PngFilterMethod.Adaptive,
            //    Gamma = 2.2f
            //});

            var font = SystemFonts.CreateFont("Oswald Medium", 12);

            if (p_renderParameters.PrintRenderData)
            {
                var runtimeString = GetRuntimeString(stopWatch.ElapsedMilliseconds);

                //var parallelString = RenderConstants.UseParallelProcessing ? "Parallel" : "Not Parallel";

                using var imageWithRunData = image.Clone(p_ctx => p_ctx.ApplyScalingWaterMark(font, $@"{p_renderParameters.XResolution}x{p_renderParameters.YResolution} | {p_renderParameters.NumberOfSamples}spp | {runtimeString}", Rgba32.GhostWhite, Rgba32.DarkSlateGray, 5, false, 30));
                if (!Directory.Exists(Path.GetDirectoryName(p_renderParameters.SavePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(p_renderParameters.SavePath));
                }

                imageWithRunData.Save(p_renderParameters.SavePath);
            }
            else
            {
                image.Save(p_renderParameters.SavePath);
            }

            // Open rendered image automatically.
            // new Process { StartInfo = new ProcessStartInfo(p_renderParameters.SavePath) { UseShellExecute = true } }.Start();

            p_renderTime = stopWatch.Elapsed;

            RandomPool.RandomPoolLUT.Clear();

            OnPropertyChanged(nameof(ImageBuffer));

            IsRendering = false;
        }

        private void RenderSomeChunk(Image<Rgba32> p_image, ICamera p_camera, IHitTarget p_world, int p_renderChunkStartRow, int p_renderChunkEndRow, RenderParameters p_renderParameters)
        {
            var rng = new Random();

            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            RandomPool.RandomPoolLUT.TryAdd(Thread.CurrentThread.ManagedThreadId, rng);

            for (var j = p_renderChunkStartRow; j <= p_renderChunkEndRow; ++j)
            {
                for (var i = 0; i < p_renderParameters.XResolution; ++i)
                {
                    var color = new Color(0, 0, 0);
                    for (var s = 0; s < p_renderParameters.NumberOfSamples; ++s)
                    {
                        var u = (i + rng.NextDouble()) / p_renderParameters.XResolution;
                        var v = (j + rng.NextDouble()) / p_renderParameters.YResolution;

                        var ray = p_camera.GetRay(u, v);

                        color += GetColor(ref ray, p_world, p_renderParameters.BounceDepth);
                    }

                    color /= p_renderParameters.NumberOfSamples;

                    color.GammaCorrect(p_renderParameters.GammaCorrection);

                    color.Clamp();

                    // Flip image writing here for Y axis. - Comment by Matt Heimlich on 11/8/2019 @ 19:24:07
                    if (p_renderParameters.RealTimeUpdate)
                    {
                        lock (ImageLock.Lock)
                        {
                            ImageBuffer.Image.SetPixel(i, p_renderParameters.YResolution - (j + 1), System.Drawing.Color.FromArgb(255, (int)(255 * color.R), (int)(255 * color.G), (int)(255 * color.B)));
                        }
                    }

                    p_image[i, p_renderParameters.YResolution - (j + 1)] = new Rgba32(new Vector3((float) color.R, (float) color.G, (float) color.B));
                }

                lock (ImageLock.Lock)
                {
                    ProcessedPixels += p_renderParameters.XResolution;

                    //if (p_renderParameters.RealTimeUpdate)
                    //{
                    //    OnPropertyChanged(nameof(ImageBuffer));
                    //}
                }
            }
        }

        private void BufferRefreshEvent(object p_sender, ElapsedEventArgs p_e)
        {
            OnPropertyChanged(nameof(ImageBuffer));
        }

        private static string GetRuntimeString(long p_stopWatchElapsedMilliseconds)
        {
            if (p_stopWatchElapsedMilliseconds < 1000)
            {
                return $@"{p_stopWatchElapsedMilliseconds}ms";
            }
            if (p_stopWatchElapsedMilliseconds >= 1000 && p_stopWatchElapsedMilliseconds < 60000)
            {
                var printSeconds = p_stopWatchElapsedMilliseconds / 1000;
                var printMilliseconds = p_stopWatchElapsedMilliseconds % 1000;
                return $@"{printSeconds}s {printMilliseconds}ms";
            }
            if (p_stopWatchElapsedMilliseconds >= 60000 && p_stopWatchElapsedMilliseconds < 3600000)
            {
                var printMinutes = p_stopWatchElapsedMilliseconds / 60000;
                var printSeconds = p_stopWatchElapsedMilliseconds % 60000 / 1000;
                var printMilliseconds = p_stopWatchElapsedMilliseconds % 60000 % 1000;
                return $@"{printMinutes}m {printSeconds}s {printMilliseconds}ms";
            }

            if (p_stopWatchElapsedMilliseconds >= 3600000)
            {
                var printHours = p_stopWatchElapsedMilliseconds / 3600000;
                var printMinutes = p_stopWatchElapsedMilliseconds % 3600000 / 60000;
                var printSeconds = p_stopWatchElapsedMilliseconds % 60000 / 1000;
                var printMilliseconds = p_stopWatchElapsedMilliseconds % 60000 % 1000;
                return $@"{printHours}h {printMinutes}m {printSeconds}s {printMilliseconds}ms";
            }

            throw new Exception("Some unknown stopwatch value encountered.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string p_propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p_propertyName));
        }
    }
}
