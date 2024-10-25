using Avans_PokeBattles.Server;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Avans_PokeBattles.Client
{
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
            this.playerName = name;
            lblName.Content = "Name: " + this.playerName;
        }

        private void SelectLobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lobbyManager.GetLobbyList();
        }

        private async void btnJoinLobby1_Click(object sender, RoutedEventArgs e) => await JoinLobby("Lobby-1", 1);
        private async void btnJoinLobby2_Click(object sender, RoutedEventArgs e) => await JoinLobby("Lobby-2", 2);
        private async void btnJoinLobby3_Click(object sender, RoutedEventArgs e) => await JoinLobby("Lobby-3", 3);

        private async Task JoinLobby(string lobbyId, int number)
        {
            lobbyNumber = number;

            // Send a join-lobby request to the server in a format it can recognize
            byte[] buffer = Encoding.UTF8.GetBytes($"join-lobby:{lobbyId}");
            await stream.WriteAsync(buffer, 0, buffer.Length);

            // Now wait for the "start-game" signal from the server
            await WaitForGameStart();
        }

        /// <summary>
        /// Wait for the server to signal the game start.
        /// </summary>
        private async Task WaitForGameStart()
        {
            byte[] buffer = new byte[10000];
            while (tcpClient.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received message from server: {message}");

                if (message == "lobby-joined")
                {
                    Console.WriteLine("Joined lobby successfully. Waiting for other player...");
                }
                else if (message == "lobby-full")
                {
                    Console.WriteLine("Lobby is full. Waiting for the game to start...");
                }
                else if (message.StartsWith("start-game"))
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var gameWindow = new LobbyWindow(tcpClient);
                        gameWindow.Show();
                        this.Close();
                    });
                    break;
                }
            }
            Console.WriteLine("Connection closed or no more messages from server.");
        }
    }
}
