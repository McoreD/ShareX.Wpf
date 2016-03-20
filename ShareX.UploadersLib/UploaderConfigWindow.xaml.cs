using HelpersLib;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ShareX.UploadersLib
{
    /// <summary>
    /// Interaction logic for UploaderConfigWindow.xaml
    /// </summary>
    public partial class UploaderConfigWindow : Window
    {
        public UploaderConfigWindow()
        {
            InitializeComponent();

            if (Uploader.PluginManager.Plugins != null)
            {
                foreach (var plugin in Uploader.PluginManager.Plugins)
                {
                    IShareXUploaderPlugin uploader = plugin.Value;

                    LeftDrawerContentItem lbItem = new LeftDrawerContentItem()
                    {
                        Name = uploader.Name,
                        Content = uploader.UI
                    };

                    lbDrawer.Items.Add(lbItem);
                }
            }
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Uploader.PluginManager.SaveSettings();
        }
    }
}