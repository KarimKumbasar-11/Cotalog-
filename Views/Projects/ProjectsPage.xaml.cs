using Cotalog.Models;
using Cotalog.Services;
using System.Windows.Controls;

namespace Cotalog.Views.Projects
{
    public partial class ProjectsPage : Page
    {
        public ProjectsPage()
        {
            InitializeComponent();
            LoadProjects();
        }

        private void LoadProjects()
        {
            ProjectsList.ItemsSource = DatabaseService.GetProjects();
        }
    }
}