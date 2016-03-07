using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HelpersLib
{
    public class ClipboardHelper
    {
        public static void SetImage(BitmapSource img)
        {
            // Create a white background render bitmap
            int dWidth = (int)img.Width;
            int dHeight = (int)img.Height;
            int dStride = dWidth * 4;
            byte[] pixels = new byte[dHeight * dStride];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = 0xFF;
            }

            BitmapSource bg = BitmapSource.Create(
                dWidth,
                dHeight,
                img.DpiX,
                img.DpiY,
                PixelFormats.Pbgra32,
                null,
                pixels,
                dStride
            );

            // Adding those two render bitmap to the same drawing visual
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            dc.DrawImage(bg, new Rect(0, 0, img.Width, img.Height));
            dc.DrawImage(img, new Rect(0, 0, img.Width, img.Height));
            dc.Close();

            // Render the result
            RenderTargetBitmap resultBitmap = new RenderTargetBitmap((int)img.Width, (int)img.Height, img.DpiX, img.DpiY, PixelFormats.Pbgra32);
            resultBitmap.Render(dv);

            // Copy it to clipboard
            try
            {
                Clipboard.SetImage(resultBitmap);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            };
        }
    }
}