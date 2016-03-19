using HelpersLib;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ScreenCaptureLib;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
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

            TaskEx.Run(() =>
            {
                Uploader.PluginManager = new UploaderPluginsManager();
                Uploader.PluginManager.Init(App.UploadersFolderPath);
            });

            editor.ImageLoaded += Editor_ImageLoaded;

            spAnnotationBar.IsEnabled = spBottomBar.IsEnabled = false;

            var annList = Enum.GetValues(typeof(AnnotationMode)).Cast<AnnotationMode>().ToList();

            foreach (AnnotationMode ann in annList)
            {
                Button btnAnnotate = new Button();
                btnAnnotate.Tag = ann;
                btnAnnotate.Click += btnAnnotate_Click;
                btnAnnotate.Width = btnAnnotate.Height = 40;
                btnAnnotate.Padding = new Thickness(0);
                btnAnnotate.Margin = new Thickness(5);
                btnAnnotate.ToolTip = ann.GetDescription();
                spAnnotationBar.Children.Add(btnAnnotate);

                switch (ann)
                {
                    case AnnotationMode.Cursor:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.CursorDefaultOutline };
                        break;
                    case AnnotationMode.Highlight:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.WeatherSunny };
                        break;
                    case AnnotationMode.Obfuscate:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.Texture };
                        break;
                    case AnnotationMode.Rectangle:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.CheckboxBlankOutline };
                        break;
                    case AnnotationMode.Ellipse:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.CheckboxBlankCircleOutline };
                        break;
                    case AnnotationMode.Line:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.VectorLine };
                        break;
                    case AnnotationMode.Arrow:
                        btnAnnotate.Content = new PackIcon() { Kind = PackIconKind.CallMade };
                        break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Editor_ImageLoaded()
        {
            spAnnotationBar.IsEnabled = spBottomBar.IsEnabled = true;
        }

        #region Capture

        private void btnCaptureArea_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Thread.Sleep(300);

            RectangleLight crop = new RectangleLight();
            if (crop.ShowDialog() == true)
            {
                editor.LoadImage(crop.GetScreenshot());
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

            editor.LoadImage(ScreenshotHelper.CaptureWindowTransparent(handle));
        }

        private void btnCaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Thread.Sleep(300);

            editor.LoadImage(ScreenshotHelper.CaptureFullscreen());

            WindowState = WindowState.Normal;
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
                editor.LoadImage(new ImageCapture(dlg.FileName));
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

            if (btnUpload.ContextMenu == null)
            {
                btnUpload.ContextMenu = new ContextMenu();
                foreach (var plugin in Uploader.PluginManager.Plugins)
                {
                    btnUpload.ContextMenu.Items.Add(new MenuItem() { Header = plugin.Value.Name });
                }
            }

            btn.SetContextMenuOnMouseDown(e);
        }

        #endregion Export

        private void btnDestinations_Click(object sender, RoutedEventArgs e)
        {
            UploaderConfigWindow dlg = new UploaderConfigWindow();
            dlg.Show();
        }

        private void btnUpload_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
        }
    }
}