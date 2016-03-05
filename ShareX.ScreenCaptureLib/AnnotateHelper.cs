using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public static class AnnotateHelper
    {
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
    }
}