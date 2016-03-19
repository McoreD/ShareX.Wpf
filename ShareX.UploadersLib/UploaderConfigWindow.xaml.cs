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
        private UploaderPluginsManager PluginManager = null;

        public UploaderConfigWindow()
        {
            InitializeComponent();

            PluginManager = new UploaderPluginsManager(Uploader.UploadersFolderPath);

            if (PluginManager.Plugins != null)
            {
                foreach (var plugin in PluginManager.Plugins)
                {
                    IShareXUploaderPlugin uploader = plugin.Value;
                    LeftDrawerContentItem o = new LeftDrawerContentItem() { Name = uploader.Name };
                    o.Content = uploader.UI;
                    lbDrawer.Items.Add(o);
                }
            }
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            PluginManager.SaveSettings();
        }
    }
}