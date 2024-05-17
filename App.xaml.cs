using System.Configuration;
using System.Data;
using System.Windows;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Set the startup URI to Login.xaml
            StartupUri = new System.Uri("Authentication.xaml", System.UriKind.Relative);
        }
    }

}
