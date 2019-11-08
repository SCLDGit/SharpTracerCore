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

namespace RenderHandler
{
    public class Renderer
    {
        public Task Render(RenderParameters p_renderParameters)
        {
            using var image = new Image<Rgba32>(p_renderParameters.XResolution, p_renderParameters.YResolution);
            var stopWatch = Stopwatch.StartNew();

            for (var j = 0; j < p_renderParameters.YResolution; ++j)
            {
                for (var i = 0; i < p_renderParameters.YResolution; ++i)
                {
                    var color = new Color(0, 0, 0);

                    color.SetR(i / (double)p_renderParameters.XResolution);
                    color.SetG(j / (double)p_renderParameters.YResolution);
                    color.SetB(0.2d);

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
