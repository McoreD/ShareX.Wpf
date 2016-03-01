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

            RectangleLight rectangleLight = new RectangleLight();
            if (rectangleLight.ShowDialog() == true)
            {
                image.Source = rectangleLight.GetAreaImage();
            }
        }

        private void btnCaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            image.Source = ScreenshotHelper.CaptureFullscreen();
        }

        #endregion Capture

        #region Editor

        private void btnEditHighlight_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnEditRotate_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion Editor

        #region Release

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.SetImage(image.Source as BitmapSource);
        }

        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Png files (*.png)|*.png";
            dlg.FileName = DateTime.Now.ToString("yyyy-MM-dd");

            if (dlg.ShowDialog() == true)
            {
                using (var fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image.Source as BitmapSource));
                    encoder.Save(fs);
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