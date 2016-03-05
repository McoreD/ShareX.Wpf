using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    // Class to hold Screenshots or other images

    public class ImageEx : Capture
    {
        public BitmapSource Source { get; private set; }

        public ObservableCollection<Shape> Annotations { get; set; } = new ObservableCollection<Shape>();

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

            dc.PushOpacity(0.5);

            foreach (var ann in Annotations)
            {
                if (ann.GetType() == typeof(Highlight))
                {
                    Highlight highlight = ann as Highlight;
                    dc.DrawRectangle(highlight.Brush, null, new Rect(new Point(highlight.X1, highlight.Y1), new Point(highlight.X2, highlight.Y2)));
                }
                else if (ann.GetType() == typeof(Obfuscate))
                {
                    Obfuscate obfuscate = ann as Obfuscate;
                    dc.DrawRectangle(obfuscate.Brush, null, new Rect(new Point(obfuscate.X1, obfuscate.Y1), new Point(obfuscate.X2, obfuscate.Y2))); ;
                }
            }

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