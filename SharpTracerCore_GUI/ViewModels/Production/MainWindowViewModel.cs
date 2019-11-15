using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

using JetBrains.Annotations;
using RenderHandler;

namespace SharpTracerCore.ViewModels.Production
{
    [MetadataType(typeof(MetaData))]
    public class MainWindowViewModel
    {
        public class MetaData : IMetadataProvider<MainWindowViewModel>
        {
            void IMetadataProvider<MainWindowViewModel>.BuildMetadata
                (MetadataBuilder<MainWindowViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.StartRender()).CommandName("StartRenderCommand");
            }

        }

        protected MainWindowViewModel()
        {
            Models.GlobalResources.ViewModels.MainWindowViewModel = this;

            RenderStatus = "Waiting...";

            Renderer = new Renderer();

            XResolution = 200;
            YResolution = 100;

            NumberOfSamples = 8;

            Parallel = true;

            BounceDepth = 12;

            GammaCorrection = 2;

            SaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SharpTracerCoreRenders", $"Render_{DateTime.Now:MMddyyyy_HHmmss}.png");
        }

        [UsedImplicitly]
        public static MainWindowViewModel Create()
        {
            return ViewModelSource.Create(() => new MainWindowViewModel());
        }

        public virtual Renderer Renderer { get; set; }

        public virtual int XResolution { get; set; }
        public virtual int YResolution { get; set; }
        public virtual int NumberOfSamples { get; set; }
        public virtual int GammaCorrection { get; set; }
        public virtual int BounceDepth { get; set; }
        public virtual bool Parallel { get; set; }
        public virtual string SaveFilePath { get; set; }

        public virtual string RenderStatus { get; set; }

        public async void StartRender()
        {
            Renderer.TotalPixels = XResolution * YResolution;

            Renderer.ProcessedPixels = 0;

            RenderStatus = "Rendering...";

            var renderParameters = new RenderParameters(XResolution, YResolution, NumberOfSamples, BounceDepth, Parallel, GammaCorrection, SaveFilePath);

            var renderTime = new TimeSpan();

            //Renderer.DoRender(renderParameters, out renderTime);

            await Task.Factory.StartNew(() => Renderer.DoRender(renderParameters, out renderTime));

            RenderStatus = $"Done! Render Time: {renderTime.Hours:00}:{renderTime.Minutes:00}:{renderTime.Seconds:00}.{renderTime.Milliseconds / 10:000}";
        }
    }
}
