using FishStore.UserAuthenticaiton;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FishStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var login = new Login();
            login.Show();
        }

    }

}
