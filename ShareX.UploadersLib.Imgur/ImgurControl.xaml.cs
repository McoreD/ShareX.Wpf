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

namespace ShareX.UploadersLib.Imgur
{
    /// <summary>
    /// Interaction logic for Imgur.xaml
    /// </summary>
    public partial class ImgurControl : UserControl
    {
        public ImgurControl()
        {
            InitializeComponent();
            LoadUI();

            oauth.OpenAuthorizePageClick += OAuth_OpenAuthorizePageClick;
            oauth.CompleteAuthorizationClick += OAuth_CompleteAuthorizationClick;
        }

        private void OAuth_OpenAuthorizePageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OAuth2Info oauth = new OAuth2Info(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);

                string url = new ImgurUploader(oauth).GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    ImgurUploader.Config.ImgurOAuth2Info = oauth;
                    URLHelper.OpenURL(url);
                    DebugHelper.WriteLine("ImgurAuthOpen - Authorization URL is opened: " + url);
                }
                else
                {
                    DebugHelper.WriteLine("ImgurAuthOpen - Authorization URL is empty.");
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
                if (!string.IsNullOrEmpty(code) && ImgurUploader.Config.ImgurOAuth2Info != null)
                {
                    bool result = new ImgurUploader(ImgurUploader.Config.ImgurOAuth2Info).GetAccessToken(code);

                    if (result)
                    {
                        oauth.Status = OAuthLoginStatus.LoginSuccessful;
                        MessageBox.Show("Login was successful.", "ShareX", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        oauth.Status = OAuthLoginStatus.LoginFailed;
                        MessageBox.Show("Login was successful but failed to get account information", "ShareX",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    // btnImgurRefreshAlbumList.Enabled = result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ShareX - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUI()
        {
            chkImgurDirectLink.IsChecked = ImgurUploader.Config.DirectLink;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveUI();
        }

        private void SaveUI()
        {
            ImgurUploader.Config.DirectLink = (bool)chkImgurDirectLink.IsChecked;
        }
    }
}