using HelpersLib;
using Microsoft.Win32;
using ShareX.ScreenCaptureLib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"ShareX 11.0";
        }

        #region Capture

        private void btnCaptureArea_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            System.Threading.Thread.Sleep(500);

            RectangleLight crop = new RectangleLight();
            if (crop.ShowDialog() == true)
            {
                editor.CapturedImage = crop.GetScreenshot();
            }

            this.WindowState = WindowState.Normal;
        }

        private void btnCaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            editor.CapturedImage = ScreenshotHelper.CaptureFullscreen();
        }

        #endregion Capture

        #region Editor

        private void btnEditHighlight_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
            {
                editor.Init(AnnotationMode.Highlight);
            }
        }

        private void btnEditObfuscate_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
            {
                editor.Init(AnnotationMode.Obfuscate);
            }
        }

        private void btnEditRotate_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
            {
                // stuff
            }
        }

        private void btnEditArrow_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
            {
                editor.Init(AnnotationMode.Arrow);
            }
        }

        #endregion Editor

        #region Release

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (editor.CapturedImage != null)
                ClipboardHelper.SetImage(editor.CapturedImage.Export());
        }

        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Png files (*.png)|*.png";
            dlg.FileName = DateTime.Now.ToString("yyyy-MM-dd");

            if (dlg.ShowDialog() == true)
            {
                using (var ms = editor.CapturedImage.ExportAsMemoryStream())
                using (var fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                {
                    ms.CopyTo(fs);
                }
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu cm = btn.ContextMenu;
            cm.PlacementTarget = btn;
            cm.IsOpen = true;
            e.Handled = true;
        }

        #endregion Release

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}