using Cotalog.Services;
using MySql.Data.MySqlClient;
using System.Windows;

namespace Cotalog.Views.Auth
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            try
            {
                var salt = SecurityHelper.GenerateSalt();
                var hashedPassword = SecurityHelper.HashPassword(txtPassword.Password, salt);

                using var connection = new MySqlConnection(DatabaseService.ConnectionString);
                connection.Open();

                var cmd = new MySqlCommand(
                    "INSERT INTO Users (login, password_hash, salt, full_name) " +
                    "VALUES (@login, @password, @salt, @fullName)",
                    connection);

                cmd.Parameters.AddWithValue("@login", txtLogin.Text);
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@fullName", txtFullName.Text);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Регистрация успешна!");
                    Close();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Логин уже занят!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}