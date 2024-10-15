using System.Configuration;
using System.Data;
using System.Windows;
using Avans_PokeBattles.Server;

namespace Avans_PokeBattles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new Server.Server();
        }
    }
}
