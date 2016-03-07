using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShareX.ScreenCaptureLib
{
    // Class to hold Screenshots or other images

    public class ImageCapture : Capture
    {
        public BitmapSource Source { get; private set; }

        private ImageCapture()
        {
            DateTimeCaptured = DateTime.Now;
        }

        public ImageCapture(BitmapSource img)
            : this()
        {
            Source = img;
        }

        public ImageCapture(string fp)
            : this()
        {
            if (File.Exists(fp))
            {
                Source = new BitmapImage(new Uri(fp));
                FilePath = fp;
            }
        }
    }
}