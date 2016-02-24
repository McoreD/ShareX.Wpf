using HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public static class Screenshot
    {
        public static BitmapSource CaptureFullscreen()
        {
            using (var screenBmp = new Bitmap(CaptureHelpers.ScreenWidth, CaptureHelpers.ScreenHeight, PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(screenBmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}