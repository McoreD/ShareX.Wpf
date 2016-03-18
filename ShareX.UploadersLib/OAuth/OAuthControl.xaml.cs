using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ShareX.UploadersLib
{
    /// <summary>
    /// Interaction logic for OAuthControl.xaml
    /// </summary>
    public partial class OAuthControl : UserControl
    {
        public event RoutedEventHandler OpenAuthorizePageClick;

        public delegate void CompleteAuthorizationClickEventHandler(string code);
        public event CompleteAuthorizationClickEventHandler CompleteAuthorizationClick;

        private OAuthLoginStatus status;
        [DefaultValue(OAuthLoginStatus.LoginRequired)]
        public OAuthLoginStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;

                switch (status)
                {
                    case OAuthLoginStatus.LoginRequired:
                        lblLoginStatus.Text = "Not logged in";
                        break;
                    case OAuthLoginStatus.LoginSuccessful:
                        lblLoginStatus.Text = "Logged in";
                        break;
                    case OAuthLoginStatus.LoginFailed:
                        lblLoginStatus.Text = "Login failed";
                        break;
                }

                txtVerificationCode.Clear();
                btnClearAuthorization.IsEnabled = btnRefreshAuthorization.IsEnabled = status == OAuthLoginStatus.LoginSuccessful;
            }
        }

        public OAuthControl()
        {
            InitializeComponent();
        }

        private void btnOpenAuthorizePage_Click(object sender, RoutedEventArgs e)
        {
            if (OpenAuthorizePageClick != null) OpenAuthorizePageClick(sender, e);
        }

        private void btnCompleteAuthorization_Click(object sender, RoutedEventArgs e)
        {
            string code = txtVerificationCode.Text;

            if (CompleteAuthorizationClick != null && !string.IsNullOrEmpty(code))
            {
                CompleteAuthorizationClick(code);
            }
        }
    }
}