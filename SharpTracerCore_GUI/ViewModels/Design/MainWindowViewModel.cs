using System;
using System.IO;
using DevExpress.Mvvm.POCO;

using JetBrains.Annotations;

namespace SharpTracerCore.ViewModels.Design
{
    public class MainWindowViewModel
    {
        protected MainWindowViewModel()
        {
            XResolution = 256;
            YResolution = 256;

            SaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SharpTracerCoreRenders");
        }

        [UsedImplicitly]
        public static MainWindowViewModel Create()
        {
            return ViewModelSource.Create(() => new MainWindowViewModel());
        }

        public virtual int XResolution { get; set; }
        public virtual int YResolution { get; set; }
        public virtual string SaveFilePath { get; set; }
    }
}
