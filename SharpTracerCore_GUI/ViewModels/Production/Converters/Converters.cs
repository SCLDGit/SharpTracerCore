using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SharpTracerCore_GUI.ViewModels.Production.Converters
{
    public class ConvertBitmapToBitmapImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is System.Drawing.Bitmap rawBitmap)) return null;
            using var memory = new MemoryStream();

            var success = false;

            while (!success)
            {
                try
                {
                    rawBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    success = true;
                }
                catch (Exception e)
                {
                }
            }

            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
