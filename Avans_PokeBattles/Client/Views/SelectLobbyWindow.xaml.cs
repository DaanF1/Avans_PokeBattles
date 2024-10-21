using Avans_PokeBattles.Server;
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
using System.Windows.Threading;
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
        private LobbyManager lobbyManager;
        private string playerName = "";
        private int lobbyNumber;

        public SelectLobbyWindow(string name, TcpClient client)
        {
            InitializeComponent();

            // Set up the TCP client and network stream with the provided client
            this.tcpClient = client;
            this.stream = client.GetStream();
            this.lobbyManager = Server.Server.GetLobbymanager();
            // Load in Player name
            this.playerName = name;
            lblName.Content = "Name: " + this.playerName;
        }

        private void SelectLobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lobbyManager.GetLobbyList();
        }

        private void btnJoinLobby1_Click(object sender, RoutedEventArgs e)
        {
            JoinLobby("Lobby-1");
        }

        private void btnJoinLobby2_Click(object sender, RoutedEventArgs e)
        {
            JoinLobby("Lobby-2");
        }

        private void btnJoinLobby3_Click(object sender, RoutedEventArgs e)
        {
            JoinLobby("Lobby-3");
        }

        private void JoinLobby(string lobbyId)
        {
            // Try joining the lobby
            bool joined = lobbyManager.TryJoinLobby(lobbyId, tcpClient);
            if (joined)
            {
                // Send joining the lobby to the server
                byte[] buffer = Encoding.ASCII.GetBytes($"{this.playerName} joined {lobbyId}");
                stream.Write(buffer, 0, buffer.Length);

                // Start waiting for the lobby to be full
                Task.Run(() =>
                {
                    WaitingInLobby(lobbyNumber);
                });
            }
            else
            {
                MessageBox.Show("Failed to join the lobby.");
            }
        }

        private int GetLobbyNumberFromId(string lobbyId)
        {
            return int.Parse(lobbyId.Split('-')[1]);
        }

        /// <summary>
        /// When joining a lobby, wait for another player to join
        /// </summary>
        /// <returns></returns>
        private async Task WaitingInLobby(int lobbyNumber)
        {
            while (true)
            {
                bool lobbyIsFull = lobbyManager.IsLobbyFull(lobbyNumber);
                if (!lobbyIsFull)
                    continue;

                // Dispatcher.Invoke because we are still on the main thread
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                    // Show the game window and close the current window
                    var gameWindow = new LobbyWindow(tcpClient);
                    gameWindow.Show();
                    this.Close();
                }));
                break;
            }
        }

    }
}
