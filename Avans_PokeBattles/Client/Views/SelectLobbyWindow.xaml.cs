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
            // Try joining the lobby
            lobbyNumber = 1;
            bool joined = lobbyManager.TryJoinLobby("Lobby-1", tcpClient);
            if (joined)
            {
                // Send joining the lobby to the server
                byte[] buffer = Encoding.ASCII.GetBytes($"{this.playerName} joined lobby {lobbyNumber}");
                stream.Write(buffer, 0, buffer.Length);

                Task.Run(() =>
                {
                    WaitingInLobby(lobbyNumber);
                });
            }
        }

        private void btnJoinLobby2_Click(object sender, RoutedEventArgs e)
        {
            // Try joining the lobby
            lobbyNumber = 2;
            bool joined = lobbyManager.TryJoinLobby("Lobby-2", tcpClient);
            if (joined)
            {
                // Send joining the lobby to the server
                byte[] buffer = Encoding.ASCII.GetBytes($"{this.playerName} joined lobby {lobbyNumber}");
                stream.Write(buffer, 0, buffer.Length);

                Task.Run(() =>
                {
                    WaitingInLobby(lobbyNumber);
                });
            }
        }

        private void btnJoinLobby3_Click(object sender, RoutedEventArgs e)
        {
            // Try joining the lobby
            lobbyNumber = 3;
            bool joined = lobbyManager.TryJoinLobby("Lobby-3", tcpClient);
            if (joined)
            {
                // Send joining the lobby to the server
                byte[] buffer = Encoding.ASCII.GetBytes($"{this.playerName} joined lobby {lobbyNumber}");
                stream.Write(buffer, 0, buffer.Length);

                Task.Run(() =>
                {
                    WaitingInLobby(lobbyNumber);
                });
            }
        }

        /// <summary>
        /// When joining a lobby, wait for another player to join
        /// </summary>
        /// <returns></returns>
        private async Task WaitingInLobby(int lobbyNumber)
        {
            while (true)
            {
                //bool lobbyIsFull = lobbyManager.IsLobbyFull(lobbyNumber);
                //if (lobbyIsFull == true)
                {
                    // Dispatcher.Invoke because we are still on the main thread
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        // Show the game window and close the current window
                        var gameWindow = new LobbyWindow();
                        gameWindow.Show();
                        this.Close();
                    }));
                    break;
                }
            }
        }

    }
}
