using HelpersLib;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public static ImageCapture CaptureRectangle(Rect rect)
        {
            if (RemoveOutsideScreenArea)
            {
                Rect bounds = CaptureHelper.GetScreenBounds();
                rect = Rect.Intersect(bounds, rect);
            }

            return new ImageCapture(CaptureRectangleNative(rect, CaptureCursor));
        }

        public static ImageCapture CaptureFullscreen()
        {
            Rect bounds = CaptureHelper.GetScreenBounds();

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
                Point cursorOffset = CaptureHelper.ScreenToClient(rect.Location);

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
            BitmapSource bmp = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DeleteObject(hBitmap);

            return bmp;
        }

        public static ImageCapture CaptureWindowTransparent(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                Rect rect = CaptureHelper.GetWindowRectangle(handle);

                if (CaptureShadow && !NativeMethods.IsZoomed(handle) && NativeMethods.IsDWMEnabled())
                {
                    rect.Inflate(ShadowOffset, ShadowOffset);
                    rect.Intersect(CaptureHelper.GetScreenBounds());
                }

                BitmapSource whiteBackground = null, blackBackground = null, whiteBackground2 = null;
                CursorData cursor = null;
                bool isTransparent = false, isTaskbarHide = false;

                try
                {
                    if (AutoHideTaskbar)
                    {
                        isTaskbarHide = NativeMethods.SetTaskbarVisibilityIfIntersect(false, rect);
                    }

                    if (CaptureCursor)
                    {
                        try
                        {
                            cursor = new CursorData();
                        }
                        catch (Exception e)
                        {
                            DebugHelper.WriteException(e, "Cursor capture failed.");
                        }
                    }

                    Window form = new Window();
                    form.Background = new SolidColorBrush(Colors.White);
                    form.WindowStyle = WindowStyle.None;
                    form.ShowInTaskbar = false;
                    form.WindowStartupLocation = WindowStartupLocation.Manual;
                    form.Left = rect.Left;
                    form.Top = rect.Top;
                    form.RenderSize = new Size(rect.Width, rect.Height);

                    IntPtr formHandle = new WindowInteropHelper(form).Handle;
                    NativeMethods.ShowWindow(formHandle, (int)WindowShowStyle.ShowNoActivate);

                    if (!NativeMethods.SetWindowPos(formHandle, handle, 0, 0, 0, 0,
                        SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE))
                    {
                        form.Close();
                        DebugHelper.WriteLine("Transparent capture failed. Reason: SetWindowPos fail.");
                        return CaptureWindow(handle);
                    }

                    Thread.Sleep(10);
                    form.UpdateLayout();

                    whiteBackground = CaptureRectangleNative(rect);

                    form.Background = new SolidColorBrush(Colors.Black);
                    form.UpdateLayout();

                    blackBackground = CaptureRectangleNative(rect);

                    form.Background = new SolidColorBrush(Colors.White);
                    form.UpdateLayout();

                    whiteBackground2 = CaptureRectangleNative(rect);

                    form.Close();

                    return new ImageCapture(whiteBackground2);

                    // TODO: Transparent window - with WPF alternative to UnsafeBitmap

                    /*
                    BitmapSource transparentImage;

                    if (whiteBackground.IsEqual(whiteBackground2))
                    {
                        transparentImage = CreateTransparentImage(whiteBackground, blackBackground);
                        isTransparent = true;
                    }
                    else
                    {
                        DebugHelper.WriteLine("Transparent capture failed. Reason: Images not equal.");
                        transparentImage = whiteBackground2;
                    }

                    if (cursor != null && cursor.IsVisible)
                    {
                        Point cursorOffset = CaptureHelper.ScreenToClient(rect.Location);
                        cursor.DrawCursorToImage(transparentImage, cursorOffset);
                    }

                    if (isTransparent)
                    {
                        transparentImage = TrimTransparent(transparentImage);

                        if (!CaptureShadow)
                        {
                            TrimShadow(transparentImage);
                        }
                    }

                    return transparentImage;
                    */
                }
                finally
                {
                    if (isTaskbarHide)
                    {
                        NativeMethods.SetTaskbarVisibility(true);
                    }

                    if (cursor != null) cursor.Dispose();
                }
            }

            return null;
        }

        public static ImageCapture CaptureWindow(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                Rect rect;

                if (CaptureClientArea)
                {
                    rect = NativeMethods.GetClientRect(handle);
                }
                else
                {
                    rect = CaptureHelper.GetWindowRectangle(handle);
                }

                bool isTaskbarHide = false;

                try
                {
                    if (AutoHideTaskbar)
                    {
                        isTaskbarHide = NativeMethods.SetTaskbarVisibilityIfIntersect(false, rect);
                    }

                    return CaptureRectangle(rect);
                }
                finally
                {
                    if (isTaskbarHide)
                    {
                        NativeMethods.SetTaskbarVisibility(true);
                    }
                }
            }

            return null;
        }
    }
}