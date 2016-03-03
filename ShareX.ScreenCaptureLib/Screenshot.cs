using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public class Screenshot : Capture
    {
        public BitmapImage Source { get; private set; } = new BitmapImage();

        public ObservableCollection<Highlight> Highlights { get; set; } = new ObservableCollection<Highlight>();

        public Screenshot(BitmapImage img)
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

            foreach (Highlight highlight in Highlights)
            {
                var rect = highlight.Rectangle;
                dc.DrawRectangle(highlight.Color, null, new Rect(highlight.TopLeft.X, highlight.TopLeft.Y, rect.Width, rect.Height));
            }

            dc.Close();
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Source.Width, (int)Source.Height, Source.DpiX, Source.DpiY, PixelFormats.Pbgra32);
            rtb.Render(dv);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);
            stream.Position = 0;

            return stream;
        }

        public BitmapImage Export()
        {
            var img = new BitmapImage();
            using (MemoryStream ms = ExportAsMemoryStream())
            {
                img.BeginInit();
                img.StreamSource = ms;
                img.EndInit();
            }
            return img;
        }
    }
}