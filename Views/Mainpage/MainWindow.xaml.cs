using Cotalog.Models;
using Cotalog.Views.Projects;
using System.Windows;
using System.Windows.Controls;

namespace Cotalog.Views.Main
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProjectsPage());
        }

        private void MyProjects_Click(object sender, RoutedEventArgs e)
        {
            // Реализация будет позже
            MessageBox.Show("Раздел 'Мои проекты' в разработке");
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CreateProjectPage(_currentUser));
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            // Реализация будет позже
            MessageBox.Show("Раздел 'Пользователи' в разработке");
        }
    }
}