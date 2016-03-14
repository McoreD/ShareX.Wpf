using HelpersLib;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ShareX.UploadersLib
{
    /// <summary>
    /// Interaction logic for UploaderConfigWindow.xaml
    /// </summary>
    public partial class UploaderConfigWindow : Window
    {
        private Dictionary<string, IShareXUploaderPlugin> Uploaders = new Dictionary<string, IShareXUploaderPlugin>();

        public UploaderConfigWindow()
        {
            InitializeComponent();

            ICollection<IShareXUploaderPlugin> plugins = null;
            TaskEx.Run(() =>
            {
                plugins = PluginHelper<IShareXUploaderPlugin>.LoadPlugins("Plugins");
            }, () =>
            {
                if (plugins != null)
                {
                    foreach (var uploader in plugins)
                    {
                        LeftDrawerContentItem o = new LeftDrawerContentItem() { Name = uploader.Name };
                        o.Content = uploader.UI;
                        lbDrawer.Items.Add(o);

                        Uploaders.Add(uploader.Name, uploader);
                    }
                }
            });
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}