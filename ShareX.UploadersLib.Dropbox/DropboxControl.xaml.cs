using HelpersLib;
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

namespace ShareX.UploadersLib.Dropbox
{
    /// <summary>
    /// Interaction logic for DropboxControl.xaml
    /// </summary>
    public partial class DropboxControl : UserControl, IShareXUploaderUI
    {
        public DropboxConfig Config { get; private set; }

        public DropboxControl()
        {
            InitializeComponent();
            oauth.OpenAuthorizePageClick += OAuth_OpenAuthorizePageClick;
        }

        public void Load(UploaderConfig config)
        {
            Config = DropboxConfig.Load($"{Name}.json") as DropboxConfig;
        }

        private void OAuth_OpenAuthorizePageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OAuth2Info oauth = new OAuth2Info(APIKeys.DropboxConsumerKey, APIKeys.DropboxConsumerSecret);

                string url = new DropboxUploader(oauth).GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    Config.DropboxOAuth2Info = oauth;
                    URLHelper.OpenURL(url);
                    DebugHelper.WriteLine("DropboxAuthOpen - Authorization URL is opened: " + url);
                }
                else
                {
                    DebugHelper.WriteLine("DropboxAuthOpen - Authorization URL is empty.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}