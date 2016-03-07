using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public class ImageAnnotation : Annotation
    {
        public ImageAnnotation(BitmapSource src)
        {
            Width = src.Width;
            Height = src.Height;

            Fill = new ImageBrush(src);
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }
    }
}