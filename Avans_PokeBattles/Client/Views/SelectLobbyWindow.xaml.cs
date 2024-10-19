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
using System.Xml.Linq;

namespace Avans_PokeBattles.Client
{
    /// <summary>
    /// Interaction logic for SelectLobbyWindow.xaml
    /// </summary>
    public partial class SelectLobbyWindow : Window
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private string playerName = "";

        public SelectLobbyWindow(TcpClient client)
        {
            InitializeComponent();

            this.tcpClient = client;
            this.stream = client.GetStream();
            //this.playerName = playerName;
        }

        private void SelectLobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnJoinLobby1_Click(object sender, RoutedEventArgs e)
        {
            
            // Hide the lobby window and show the game window
            this.Hide();
            var gameWindow = new LobbyWindow(playerName);
            gameWindow.Show();
        }

        private void btnJoinLobby2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnJoinLobby3_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
