using System;
using System.Diagnostics;
using System.IO;
using Fclp;
using RenderHandler;

namespace SharpTracerCore_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var renderer = new Renderer();

            var parsedArguments =  ParseArguments(args);

            var renderParameters = new RenderParameters(parsedArguments.XResolution, parsedArguments.YResolution, parsedArguments.NumberOfSamples, parsedArguments.BounceDepth, parsedArguments.GammaCorrection, parsedArguments.SavePath);

            Console.WriteLine($@"Rendering {renderParameters.XResolution} x {renderParameters.YResolution} image to '{renderParameters.SavePath}'...");

            renderer.DoRender(renderParameters, out var renderTime);

            Console.WriteLine($"Done! Render took {renderTime.Hours:00}:{renderTime.Minutes:00}:{renderTime.Seconds:00}.{renderTime.Milliseconds / 10:000}");
        }

        private static ProgramArguments ParseArguments(string[] p_args)
        {
            var parser = new FluentCommandLineParser<ProgramArguments>();

            parser.Setup(p_arg => p_arg.XResolution).As('x', "xRes")
                .WithDescription("X resolution of rendered image.").SetDefault(2000);

            parser.Setup(p_arg => p_arg.YResolution).As('y', "yRes")
                .WithDescription("Y resolution of rendered image.").SetDefault(1000);

            parser.Setup(p_arg => p_arg.NumberOfSamples).As('s', "samples")
                .WithDescription("Number of samples per pixel.").SetDefault(8);

            parser.Setup(p_arg => p_arg.BounceDepth).As('b', "bounces")
                .WithDescription("Number of light bounces before ending tracing.").SetDefault(8);

            parser.Setup(p_arg => p_arg.GammaCorrection).As('g', "gamma")
                .WithDescription("Gamma correction value.").SetDefault(2);

            parser.Setup(p_arg => p_arg.SavePath).As('p', "path")
                .WithDescription("Full path of rendered image.").SetDefault(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SharpTracerCoreRenders", $"Render_{DateTime.Now:MMddyyyy_HHmmss}.png"));

            parser.SetupHelp("h", "help").Callback(p_helpText => Console.WriteLine($@"The following parameters are available:{Environment.NewLine}{p_helpText}"));

            var result = parser.Parse(p_args);

            if (result.HelpCalled)
            {
               Process.GetCurrentProcess().Kill();
            }

            if (result.HasErrors)
            {
                throw new Exception("Invalid command line arguments encountered.");
            }

            return parser.Object;
        }
    }
}
