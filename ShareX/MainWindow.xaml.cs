using HelpersLib;
using Microsoft.Win32;
using ScreenCaptureLib;
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = $"ShareX WPF";

            editor.ImageLoaded += Editor_ImageLoaded;

            spAnnotations.IsEnabled = false;

            var annList = Enum.GetValues(typeof(AnnotationMode)).Cast<AnnotationMode>().ToList();

            foreach (AnnotationMode ann in annList)
            {
                Button btnAnnotate = new Button() { Content = ann.GetDescription(), Tag = ann };
                btnAnnotate.Click += btnAnnotate_Click;
                btnAnnotate.Margin = new Thickness(10);
                spAnnotations.Children.Add(btnAnnotate);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Editor_ImageLoaded()
        {
            spAnnotations.IsEnabled = true;
        }

        #region Capture

        private void btnCaptureArea_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Thread.Sleep(300);

            RectangleLight crop = new RectangleLight();
            if (crop.ShowDialog() == true)
            {
                editor.CapturedImage = crop.GetScreenshot();
            }

            WindowState = WindowState.Normal;
        }

        private void btnCaptureWindow_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.SetContextMenuOnMouseDown(e);
            PrepareCaptureMenuAsync(tsmiWindow, tsmiWindowItems_Click);
        }

        private void btnCaptureWindow_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            PrepareCaptureMenuAsync(tsmiWindow, tsmiWindowItems_Click);
        }

        private void tsmiWindowItems_Click(object sender, EventArgs e)
        {
            MenuItem tsi = (MenuItem)sender;
            WindowInfo wi = tsi.Tag as WindowInfo;
            if (wi != null)
            {
                CaptureWindow(wi.Handle);
            }
        }

        private void PrepareCaptureMenuAsync(ContextMenu tsmiWindow, RoutedEventHandler handlerWindow)
        {
            tsmiWindow.Items.Clear();

            WindowsList windowsList = new WindowsList();
            List<WindowInfo> windows = null;
            windows = windowsList.GetVisibleWindowsList();

            TaskEx.Run(() =>
            {
                windows = windowsList.GetVisibleWindowsList();
            }, () =>
            {
                if (windows != null)
                {
                    foreach (WindowInfo window in windows)
                    {
                        string title = window.Text.Truncate(40, "...");
                        MenuItem mi = new MenuItem() { Header = title };
                        mi.Tag = window;
                        mi.Click += handlerWindow;
                        tsmiWindow.Items.Add(mi);
                    }
                }
            }
            );
        }

        private void CaptureWindow(IntPtr handle)
        {
            if (NativeMethods.IsIconic(handle))
            {
                NativeMethods.RestoreWindow(handle);
            }

            NativeMethods.SetForegroundWindow(handle);
            Thread.Sleep(250);

            editor.CapturedImage = ScreenshotHelper.CaptureWindowTransparent(handle);
        }

        private void btnCaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            editor.CapturedImage = ScreenshotHelper.CaptureFullscreen();
        }

        #endregion Capture

        #region Open

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            StringBuilder sbFilter = new StringBuilder();
            foreach (string ext in Enum.GetNames(typeof(EImageFormat)))
            {
                sbFilter.Append($"{ext} files|*.{ext}|");
            }

            dlg.Filter = sbFilter.ToString().TrimEnd('|');
            if (dlg.ShowDialog() == true)
            {
                editor.CapturedImage = new ImageCapture(dlg.FileName);
            }
        }

        #endregion Open

        #region Editor

        private void btnAnnotate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            editor.AnnotationMode = (AnnotationMode)btn.Tag;
        }

        #endregion Editor

        #region Export

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
                ClipboardHelper.SetImage(editor.GetBitmap());
        }

        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Png files (*.png)|*.png";
            dlg.FileName = DateTime.Now.ToString("yyyy-MM-dd");

            if (dlg.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                var outputFrame = BitmapFrame.Create(editor.GetBitmap());
                encoder.Frames.Add(outputFrame);
                using (var fp = File.OpenWrite(dlg.FileName))
                {
                    encoder.Save(fp);
                }
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.SetContextMenuOnMouseDown(e);
        }

        #endregion Export
    }
}