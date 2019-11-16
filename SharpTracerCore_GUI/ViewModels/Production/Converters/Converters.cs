using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RenderHandler;

namespace SharpTracerCore_GUI.ViewModels.Production.Converters
{
    public static class ConverterUtils
    {
        public static bool IsConverting { get; set; }
    }
    public class ConvertBitmapToBitmapImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ThreadSafeImage threadSafeImage)) return null;

            if (ConverterUtils.IsConverting) return Models.GlobalResources.ViewModels.MainWindowViewModel.WriteableImageBuffer;

            ConverterUtils.IsConverting = true;

            using var memory = new MemoryStream();

            lock (ImageLock.Lock)
            {
                threadSafeImage.Image.Save(memory, ImageFormat.Bmp);
            }

            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            ConverterUtils.IsConverting = false;

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertBitmapToWriteableBitmap : IValueConverter
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr dest, IntPtr source, int Length);

        public void GetImage(Bitmap p_bitmap, WriteableBitmap p_writeableBitmap)
        {
            var data = p_bitmap.LockBits(new Rectangle(0, 0, p_bitmap.Width, p_bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                p_writeableBitmap.Lock();
                CopyMemory(p_writeableBitmap.BackBuffer, data.Scan0,
                    (p_writeableBitmap.BackBufferStride * p_bitmap.Height));
                p_writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, p_bitmap.Width, p_bitmap.Height));
                p_writeableBitmap.Unlock();
            }
            finally
            {
                p_bitmap.UnlockBits(data);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ThreadSafeImage threadSafeImage)) return null;

            if (ConverterUtils.IsConverting) return Models.GlobalResources.ViewModels.MainWindowViewModel.WriteableImageBuffer;

            ConverterUtils.IsConverting = true;

            Bitmap copy;

            lock (ImageLock.Lock)
            {
                copy = new Bitmap(threadSafeImage.Image);
            }

            GetImage(copy, Models.GlobalResources.ViewModels.MainWindowViewModel.WriteableImageBuffer);

            ConverterUtils.IsConverting = false;

            copy.Dispose();

            return Models.GlobalResources.ViewModels.MainWindowViewModel.WriteableImageBuffer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
