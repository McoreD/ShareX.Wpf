using System;
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

    public class ImageEx : Capture
    {
        public BitmapSource Source { get; private set; }

        public ObservableCollection<Annotation> Annotations { get; set; } = new ObservableCollection<Annotation>();

        public ImageEx(BitmapSource img)
        {
            Source = img;
            DateTimeCaptured = DateTime.Now;
        }

        public MemoryStream ExportAsMemoryStream()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            dc.DrawImage(Source, new Rect(0, 0, Source.Width, Source.Height));

            foreach (var ann in Annotations) { ann.Render(dc); }

            // Parallel.ForEach(Annotations, ann => { ann.Render(dc); });

            dc.Close();

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Source.Width, (int)Source.Height, Source.DpiX, Source.DpiY, PixelFormats.Pbgra32);
            rtb.Render(dv);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream ms = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(ms);
            ms.Position = 0;

            return ms;
        }

        public BitmapImage Export()
        {
            var img = new BitmapImage();
            using (MemoryStream ms = ExportAsMemoryStream())
            {
                img.BeginInit();
                img.StreamSource = new MemoryStream(ms.ToArray());
                img.EndInit();
            }
            return img;
        }
    }
}