using HelpersLib;
using ShareX.HelpersLib;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public static partial class ScreenshotHelper
    {
        public static bool RemoveOutsideScreenArea = true;
        public static bool CaptureCursor = false;
        public static bool CaptureClientArea = false;
        public static bool CaptureShadow = true;
        public static int ShadowOffset = 20;
        public static bool AutoHideTaskbar = false;

        public static BitmapSource CaptureRectangle(Rect rect)
        {
            if (RemoveOutsideScreenArea)
            {
                Rect bounds = CaptureHelpers.GetScreenBounds();
                rect = Rect.Intersect(bounds, rect);
            }

            return CaptureRectangleNative(rect, CaptureCursor);
        }

        public static BitmapSource CaptureFullscreen()
        {
            Rect bounds = CaptureHelpers.GetScreenBounds();

            return CaptureRectangle(bounds);
        }

        public static BitmapSource CaptureRectangleNative(Rect rect, bool captureCursor = false)
        {
            return CaptureRectangleNative(NativeMethods.GetDesktopWindow(), rect, captureCursor);
        }

        public static BitmapSource CaptureRectangleNative(IntPtr handle, Rect rect, bool captureCursor = false)
        {
            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }

            IntPtr hdcSrc = NativeMethods.GetWindowDC(handle);
            IntPtr hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, (int)rect.Width, (int)rect.Height);
            IntPtr hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            NativeMethods.BitBlt(hdcDest, 0, 0, (int)rect.Width, (int)rect.Height, hdcSrc, (int)rect.X, (int)rect.Y, System.Drawing.CopyPixelOperation.SourceCopy | System.Drawing.CopyPixelOperation.CaptureBlt);

            if (captureCursor)
            {
                Point cursorOffset = CaptureHelpers.ScreenToClient(rect.Location);

                try
                {
                    using (CursorData cursorData = new CursorData())
                    {
                        cursorData.DrawCursorToHandle(hdcDest, cursorOffset);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Cursor capture failed.");
                }
            }

            NativeMethods.SelectObject(hdcDest, hOld);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(handle, hdcSrc);
            BitmapSource img = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DeleteObject(hBitmap);

            return img;
        }
    }
}