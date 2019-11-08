using System;
using System.Diagnostics;
using System.IO;
using Fclp;
using RenderHandler;

namespace SharpTracerCore_CLI
{
    public class ProgramArguments
    {
        public int XResolution { get; set; }
        public int YResolution { get; set; }
        public string SavePath { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var renderer = new Renderer();

            var parsedArguments =  ParseArguments(args);

            var renderParameters = new RenderParameters(parsedArguments.XResolution, parsedArguments.YResolution, parsedArguments.SavePath);

            Console.WriteLine($@"Rendering {renderParameters.XResolution} x {renderParameters.YResolution} image to '{renderParameters.SavePath}'...");

            var renderTask = renderer.Render(renderParameters);

            renderTask.Wait();

            Console.WriteLine("Done!");
        }

        private static ProgramArguments ParseArguments(string[] p_args)
        {
            var parser = new FluentCommandLineParser<ProgramArguments>();

            parser.Setup(p_arg => p_arg.XResolution).As('x', "xRes")
                .WithDescription("X resolution of rendered image.").SetDefault(256);

            parser.Setup(p_arg => p_arg.YResolution).As('y', "yRes")
                .WithDescription("Y resolution of rendered image.").SetDefault(256);

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
