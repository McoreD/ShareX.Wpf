using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public static class AnnotationHelper
    {
        public static ImageEx CapturedImage { get; private set; }

        public static void LoadCapturedImage(ImageEx src)
        {
            CapturedImage = src;
        }

        public static WriteableBitmap ChangeColor(BitmapSource source, Color highlightColor)
        {
            WriteableBitmap wbmp = new WriteableBitmap(source);

            for (int x = 0; x < wbmp.PixelWidth; x++)
            {
                for (int y = 0; y < wbmp.PixelHeight; y++)
                {
                    Color color = new Color()
                    {
                        A = wbmp.GetPixel(x, y).A,
                        R = wbmp.GetPixel(x, y).R,
                        G = wbmp.GetPixel(x, y).G,
                        B = wbmp.GetPixel(x, y).B,
                    };

                    color = Color.FromArgb(color.A,
                        Math.Min(highlightColor.R, color.R),
                        Math.Min(highlightColor.G, color.G),
                        Math.Min(highlightColor.B, color.B));

                    wbmp.SetPixel(x, y, color);
                }
            }

            return wbmp;
        }

        public static Rect CreateIntersectRect(Rect rect)
        {
            Rect applyRect = new Rect(0, 0, CapturedImage.Source.Width, CapturedImage.Source.Height);
            Rect myRect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
            myRect.Intersect(applyRect);
            return myRect;
        }
    }
}