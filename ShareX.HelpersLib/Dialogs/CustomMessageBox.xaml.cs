using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace HelpersLib
{
    public partial class CustomMessageBox : UserControl
    {
        public CustomMessageBox(string text, params string[] buttons)
        {
            InitializeComponent();

            tbText.Text = text;

            if (buttons == null || buttons.Length == 0)
            {
                buttons = new string[] { "Ok" };
            }

            if (buttons != null)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    Button btn = new Button();
                    btn.Content = buttons[i];
                    btn.Command = DialogHost.CloseDialogCommand;
                    btn.CommandParameter = i + 1;

                    if (buttons.Length == 1)
                        btn.IsDefault = true;

                    spButtons.Children.Add(btn);
                }
            }
        }
    }
}