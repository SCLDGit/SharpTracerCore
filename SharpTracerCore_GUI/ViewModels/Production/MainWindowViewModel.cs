using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
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

            XResolution = 256;
            YResolution = 256;

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
        public virtual string SaveFilePath { get; set; }

        public virtual string RenderStatus { get; set; }

        public void StartRender()
        {
            RenderStatus = "Rendering...";

            var renderParameters = new RenderParameters(XResolution, YResolution, SaveFilePath);

            var renderTask = Renderer.Render(renderParameters);

            renderTask.Wait();

            RenderStatus = "Done!";
        }
    }
}
