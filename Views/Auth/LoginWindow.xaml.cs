using Cotalog.Services;
using Cotalog.Views.Main;
using System.Windows;

namespace Cotalog.Views.Auth
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var user = DatabaseService.AuthenticateUser(txtLogin.Text, txtPassword.Password);
            if (user != null)
            {
                new MainWindow(user).Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().Show();
            Close();
        }
    }
}