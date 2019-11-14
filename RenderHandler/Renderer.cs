using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

using RenderDataStructures.Basics;
using RenderDataStructures.Shapes;

using JetBrains.Annotations;
using RenderDataStructures.Cameras;
using RenderDataStructures.Materials;

namespace RenderHandler
{
    public class Renderer : INotifyPropertyChanged
    {
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

        public Color GetColor(ref Ray p_ray, World p_world, int p_totalDepth)
        {
            var hitRecord = new HitRecord();

            if (p_world.WasHit(p_ray, 0.001d, double.MaxValue, ref hitRecord))
            {
                var scatteredRay = new Ray(new Vec3(0, 0, 0), new Vec3(0, 0, 0));
                var attenuation = new Color(0, 0, 0);

                if (p_ray.Depth < p_totalDepth && hitRecord.Material.ScatterRay(ref p_ray, ref hitRecord, ref attenuation, ref scatteredRay))
                {
                    return attenuation * GetColor(ref scatteredRay, p_world, p_totalDepth);
                }
                else
                {
                    return new Color(0, 0, 0);
                }
            }

            var unitDirection = Vec3.GetUnitVector(p_ray.Direction);
            var t = 0.5 * (unitDirection.Y + 1);
            return (1.0 - t) * new Color(1.0, 1.0, 1.0) + t * new Color(0.5, 0.7, 1.0);
        }

        public void DoRender(RenderParameters p_renderParameters, out TimeSpan p_renderTime)
        {
            IsRendering = true;

            using var image = new Image<Rgba32>(p_renderParameters.XResolution, p_renderParameters.YResolution);

            var stopWatch = Stopwatch.StartNew();

            var world = new World();

            world.AddTarget(new Sphere(new Vec3(0, 0, -1), 0.5, new Lambertian(new Color(0.8, 0.3, 0.3))));
            world.AddTarget(new Sphere(new Vec3(0, -100.5, -1), 100, new Lambertian(new Color(0.8, 0.8, 0.0))));
            world.AddTarget(new Sphere(new Vec3(1, 0, -1), 0.5, new Glossy(new Color(0.8, 0.6, 0.2), 1.0)));
            world.AddTarget(new Sphere(new Vec3(-1, 0, -1), 0.5, new Dielectric(new Color(1, 1, 1), 1.5)));
            world.AddTarget(new Sphere(new Vec3(-1, 0, -1), -0.45, new Dielectric(new Color(1, 1, 1), 1.5)));

            var camera = new Camera();

            var rng = new Random();

            for (var j = 0; j < p_renderParameters.YResolution; ++j)
            {
                for (var i = 0; i < p_renderParameters.XResolution; ++i)
                {
                    var color = new Color(0, 0, 0);
                    for (var s = 0; s < p_renderParameters.NumberOfSamples; ++s)
                    {
                        var u = (i + rng.NextDouble()) / p_renderParameters.XResolution;
                        var v = (j + rng.NextDouble()) / p_renderParameters.YResolution;

                        var ray = camera.GetRay(u, v);

                        color += GetColor(ref ray, world, p_renderParameters.BounceDepth);
                    }

                    color /= p_renderParameters.NumberOfSamples;

                    color.GammaCorrect(p_renderParameters.GammaCorrection);

                    // Flip image writing here for Y axis. - Comment by Matt Heimlich on 11/8/2019 @ 19:24:07
                    image[i, p_renderParameters.YResolution - (j + 1)] =
                        new Rgba32(new Vector3((float)color.R, (float)color.G, (float)color.B));

                }

                ProcessedPixels += p_renderParameters.XResolution;
            }

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
            new Process { StartInfo = new ProcessStartInfo(p_renderParameters.SavePath) { UseShellExecute = true } }.Start();

            p_renderTime = stopWatch.Elapsed;

            IsRendering = false;
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
