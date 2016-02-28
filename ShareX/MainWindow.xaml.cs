using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        #region Capture

        private void btnCaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            image.Source = Screenshot.CaptureFullscreen();
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

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu cm = btn.ContextMenu;
            cm.PlacementTarget = btn;
            cm.IsOpen = true;
            e.Handled = true;
        }

        #endregion Release
    }
}