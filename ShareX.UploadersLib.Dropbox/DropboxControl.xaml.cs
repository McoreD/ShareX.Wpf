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
    public partial class DropboxControl : UserControl
    {
        internal void UpdateDropboxStatus()
        {
            if (OAuth2Info.CheckOAuth(DropboxUploader.Config.DropboxOAuth2Info) && DropboxUploader.Config.DropboxAccountInfo != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Email: " + DropboxUploader.Config.DropboxAccountInfo.Email);
                sb.AppendLine("Name: " + DropboxUploader.Config.DropboxAccountInfo.Display_name);
                sb.AppendLine("ID: " + DropboxUploader.Config.DropboxAccountInfo.Uid.ToString());
                // string uploadPath = GetDropboxUploadPath();
                // sb.AppendLine("Upload path: " + uploadPath);
                // sb.AppendLine("Download path: " + DropboxUploader.GetPublicURL(DropboxUploader.Config.DropboxAccountInfo.Uid, uploadPath + "Example.png"));
                lblDropboxStatus.Text = sb.ToString();
                // btnDropboxShowFiles.Enabled = true;
            }
            else
            {
                lblDropboxStatus.Text = string.Empty;
            }
        }

        public DropboxControl()
        {
            InitializeComponent();

            UpdateDropboxStatus();
            chkDropboxAutoCreateShareableLink.IsChecked = DropboxUploader.Config.DropboxAutoCreateShareableLink;

            oauth.OpenAuthorizePageClick += OAuth_OpenAuthorizePageClick;
            oauth.CompleteAuthorizationClick += OAuth_CompleteAuthorizationClick;
        }

        private void OAuth_OpenAuthorizePageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OAuth2Info oauth = new OAuth2Info(APIKeys.DropboxConsumerKey, APIKeys.DropboxConsumerSecret);

                string url = new DropboxUploader(oauth).GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    DropboxUploader.Config.DropboxOAuth2Info = oauth;
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

        private void OAuth_CompleteAuthorizationClick(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code) && DropboxUploader.Config.DropboxOAuth2Info != null)
                {
                    DropboxUploader dropbox = new DropboxUploader(DropboxUploader.Config.DropboxOAuth2Info);
                    bool result = dropbox.GetAccessToken(code);

                    if (result)
                    {
                        DropboxUploader.Config.DropboxAccountInfo = dropbox.GetAccountInfo();
                        UpdateDropboxStatus();

                        oauth.Status = OAuthLoginStatus.LoginSuccessful;

                        if (DropboxUploader.Config.DropboxAccountInfo != null)
                        {
                            MessageBox.Show("Login was successful.", "ShareX", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Login was successful but failed to get account information", "ShareX",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        return;
                    }
                    else
                    {
                        oauth.Status = OAuthLoginStatus.LoginFailed;
                        MessageBox.Show("Login failed", "ShareX", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                DropboxUploader.Config.DropboxAccountInfo = null;
                UpdateDropboxStatus();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
                MessageBox.Show(ex.ToString(), "ShareX - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DropboxUploader.Config.DropboxAutoCreateShareableLink = (bool)chkDropboxAutoCreateShareableLink.IsChecked;
        }
    }
}