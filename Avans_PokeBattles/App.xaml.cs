using System.Configuration;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;

namespace Avans_PokeBattles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Start with a console
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        /// <summary>
        /// Define startup project files
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AllocConsole();

            // Start Server
            Task.Run(() =>
            {
                var server = new Server.Server();
                server.Start();
            });

            // Start Client 1
            var client1 = new Client.LoginWindow();
            client1.Show();

            // Start Client 2
            var client2 = new Client.LoginWindow();
            client2.Show();
        }
    }
}
