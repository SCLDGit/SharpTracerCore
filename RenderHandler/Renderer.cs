using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

using RenderDataStructures;
using RenderDataStructures.Basics;

namespace RenderHandler
{
    public class Renderer
    {
        public double HitSphere(Vec3 p_center, double p_radius, ref Ray p_ray)
        {
            var objectCenter = p_ray.Origin - p_center;

            var a = Vec3.GetDotProduct(p_ray.Direction, p_ray.Direction);
            var b = 2.0d * Vec3.GetDotProduct(objectCenter, p_ray.Direction);
            var c = Vec3.GetDotProduct(objectCenter, objectCenter) - p_radius * p_radius;

            var discriminant = b * b - 4 * a * c;

            return discriminant < 0 ? -1.0 : (-b - Math.Sqrt(discriminant)) / (2.0 * a);
        }

        public Color GetColor(ref Ray p_ray)
        {
            var t = HitSphere(new Vec3(0, 0, -1), 0.5d, ref p_ray);

            if (t > 0.0d)
            {
                var n = Vec3.GetUnitVector(p_ray.PointAt(t) - new Vec3(0, 0, -1));
                return 0.5 * new Color(n.X + 1, n.Y + 1, n.Z + 1);
            }

            var unitDirection = Vec3.GetUnitVector(p_ray.Direction);

            t = 0.5 * (unitDirection.Y + 1.0);

            return (1.0d - t) * new Color(1.0, 1.0, 1.0) + t * new Color(0.5, 0.7, 1.0);
        }

        public Task Render(RenderParameters p_renderParameters)
        {
            using var image = new Image<Rgba32>(p_renderParameters.XResolution, p_renderParameters.YResolution);

            var stopWatch = Stopwatch.StartNew();

            var lowerLeftCorner = new Vec3(-2.0, -1.0, -1.0);
            var horizontal = new Vec3(4.0, 0.0, 0.0);
            var vertical = new Vec3(0.0, 2.0, 0.0);
            var origin = new Vec3(0, 0, 0);

            for (var j = 0; j < p_renderParameters.YResolution; ++j)
            {
                for (var i = 0; i < p_renderParameters.XResolution; ++i)
                {
                    var u = i / (double)p_renderParameters.XResolution;
                    var v = j / (double)p_renderParameters.YResolution;
                    
                    var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical);

                    var color = GetColor(ref ray);

                    // Flip image writing here for Y axis. - Comment by Matt Heimlich on 11/8/2019 @ 19:24:07
                    image[i, p_renderParameters.YResolution - (j + 1)] =
                        new Rgba32(new Vector3((float)color.R, (float)color.G, (float)color.B));
                }
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

                using var imageWithRunData = image.Clone(p_ctx => p_ctx.ApplyScalingWaterMark(font, $@"{p_renderParameters.XResolution}x{p_renderParameters.YResolution} | {runtimeString}", Rgba32.GhostWhite, Rgba32.DarkSlateGray, 5, false, 30));
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

            return Task.CompletedTask;
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
    }
}
