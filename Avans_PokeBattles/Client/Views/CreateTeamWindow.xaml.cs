using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Avans_PokeBattles.Server;

namespace Avans_PokeBattles.Client.Views
{
    /// <summary>
    /// Interaction logic for CreateTeamWindow.xaml
    /// </summary>
    public partial class CreateTeamWindow : Window
    {
        private readonly Profile playerProfile;
        private readonly TcpClient playerClient;

        public CreateTeamWindow(Profile profile, TcpClient client)
        {
            InitializeComponent();

            this.playerProfile = profile;
            this.playerClient = client;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Go to select lobby window
            var lobbyWindow = new SelectLobbyWindow(playerProfile, playerClient);
            lobbyWindow.Show();
            this.Close();
        }

    }
}
