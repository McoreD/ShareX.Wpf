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
        private Dictionary<string, IShareXUploaderPlugin> Plugins = new Dictionary<string, IShareXUploaderPlugin>();

        public UploaderConfigWindow()
        {
            InitializeComponent();

            TaskEx.Run(() =>
            {
                Plugins = PluginHelper<IShareXUploaderPlugin>.LoadPlugins("Plugins");
            }, () =>
            {
                if (Plugins != null)
                {
                    foreach (var plugin in Plugins)
                    {
                        IShareXUploaderPlugin uploader = plugin.Value;
                        uploader.Location = plugin.Key;
                        uploader.LoadSettings(Path.ChangeExtension(uploader.Location, "json"));

                        LeftDrawerContentItem o = new LeftDrawerContentItem() { Name = uploader.Name };

                        o.Content = uploader.UI;
                        lbDrawer.Items.Add(o);
                    }
                }
            });
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var plugin in Plugins)
            {
                plugin.Value.SaveSettings();
            }
        }
    }
}