using Cotalog.Services;
using Cotalog.Views.Auth;
using System.Windows;

namespace Cotalog
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseService.InitializeDatabase();
            new LoginWindow().Show();
        }
    }
}