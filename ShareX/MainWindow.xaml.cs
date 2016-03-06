using HelpersLib;
using Microsoft.Win32;
using ScreenCaptureLib;
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                if (ann == AnnotationMode.None)
                    continue;

                Button btnAnnotate = new Button() { Content = ann.GetDescription(), Tag = ann };
                btnAnnotate.Click += btnAnnotate_Click;
                btnAnnotate.Margin = new Thickness(10);
                spAnnotations.Children.Add(btnAnnotate);
            }
        }

        private void Editor_ImageLoaded()
        {
            spAnnotations.IsEnabled = true;
        }

        private void btnAnnotate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            editor.AnnotationMode = (AnnotationMode)btn.Tag;
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

        private void btnCaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            editor.CapturedImage = ScreenshotHelper.CaptureFullscreen();
        }

        #endregion Capture

        #region Editor

        #endregion Editor

        #region Release

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
                ClipboardHelper.SetImage(editor.CapturedImage.Export(editor.Annotations));
        }

        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Png files (*.png)|*.png";
            dlg.FileName = DateTime.Now.ToString("yyyy-MM-dd");

            if (dlg.ShowDialog() == true)
            {
                using (var ms = editor.CapturedImage.ExportAsMemoryStream(editor.Annotations))
                using (var fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                {
                    ms.CopyTo(fs);
                }
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.SetContextMenuOnMouseDown(e);
        }

        #endregion Release

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
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
                        string title = window.Text.Truncate(50, "...");
                        MenuItem mi = new MenuItem() { Header = title };
                        mi.Tag = window;
                        mi.Click += handlerWindow;
                        tsmiWindow.Items.Add(mi);

                        // lbWindows.Items.Add(window);
                        //  lbWindows.Visibility = Visibility.Visible;
                    }
                }
            }
            );
        }

        private void btnCaptureWindow_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.SetContextMenuOnMouseDown(e);
            PrepareCaptureMenuAsync(tsmiWindow, tsmiWindowItems_Click);
        }

        private void lbWindows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = lbWindows.SelectedItem as WindowInfo;
            CaptureWindow(window.Handle);
        }
    }
}