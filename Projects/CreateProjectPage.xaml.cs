using Cotalog.Models;
using Cotalog.Services;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cotalog.Views.Projects
{
    public partial class CreateProjectPage : Page
    {
        private readonly User _currentUser;
        private readonly List<ProjectFile> _files = new();

        public CreateProjectPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            FilesList.ItemsSource = _files;
        }

        private void AddPhoto_Click(object sender, RoutedEventArgs e) => AddFile("photo");
        private void AddVideo_Click(object sender, RoutedEventArgs e) => AddFile("video");
        private void AddDocument_Click(object sender, RoutedEventArgs e) => AddFile("document");

        private void AddFile(string fileType)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                _files.Add(new ProjectFile
                {
                    FilePath = dialog.FileName,
                    FileName = Path.GetFileName(dialog.FileName),
                    FileType = fileType
                });
                FilesList.Items.Refresh();
            }
        }

        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is ProjectFile file)
            {
                _files.Remove(file);
                FilesList.Items.Refresh();
            }
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название проекта!");
                return;
            }

            var project = new Project
            {
                Title = txtTitle.Text,
                ShortDescription = txtShortDesc.Text,
                DetailedDescription = txtDetailedDesc.Text,
                AuthorId = _currentUser.Id,
                Files = _files
            };

            if (DatabaseService.SaveProject(project))
            {
                MessageBox.Show("Проект успешно создан!");
                NavigationService.GoBack();
            }
        }
    }
}