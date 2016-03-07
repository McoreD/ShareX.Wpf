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

namespace HelpersLib
{
    /// <summary>
    /// Interaction logic for LoginBox.xaml
    /// </summary>
    public partial class LoginBox : UserControl
    {
        public LoginBoxData Settings { get; set; } = new LoginBoxData();

        public LoginBox()
        {
            InitializeComponent();
            UserNameBox.Text = Settings.UserName;
        }

        public LoginBox(string question)
            : this()
        {
            lblQuestion.Text = question;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Settings.UserName = UserNameBox.Text;
            Settings.Password = PasswordBox.Password;
        }
    }

    public class LoginBoxData
    {
        public string UserName { get; set; } = $@".\{Environment.UserName}";
        public string Password { get; set; }
    }
}