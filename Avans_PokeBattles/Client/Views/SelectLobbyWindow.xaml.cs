using Avans_PokeBattles.Client.Views;
using Avans_PokeBattles.Server;
using System.IO.IsolatedStorage;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Avans_PokeBattles.Client
{
    public partial class SelectLobbyWindow : Window
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream stream;
        private readonly LobbyManager lobbyManager;
        private readonly string playerName = "";
        public LoadingWindow loadingWindow = new LoadingWindow("Waiting for another player."); // Make loading window accessable

        public SelectLobbyWindow(string name, TcpClient client)
        {
            InitializeComponent();

            // Set up the TCP client and network stream with the provided client
            this.tcpClient = client;
            this.stream = client.GetStream();

            // Retrieve the lobby manager instance from the server
            this.lobbyManager = Server.Server.GetLobbymanager();

            // Set the player’s name and update the label with this information
            this.playerName = name;
            lblName.Content = "Name: " + this.playerName;
        }

        // Event handler triggered when the SelectLobbyWindow is loaded
        private void SelectLobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Request the list of available lobbies from the server
            lobbyManager.GetLobbyList();
        }

        // Asynchronous event handlers for the Join Lobby buttons
        // Each button calls the JoinLobby method with a specific lobby ID and number
        private async void btnJoinLobby1_Click(object sender, RoutedEventArgs e) => await JoinLobby("Lobby-1", 1);
        private async void btnJoinLobby2_Click(object sender, RoutedEventArgs e) => await JoinLobby("Lobby-2", 2);
        private async void btnJoinLobby3_Click(object sender, RoutedEventArgs e) => await JoinLobby("Lobby-3", 3);


        // Method to handle joining a specified lobby
        private async Task JoinLobby(string lobbyId, int number)
        {
            // Prepare the join-lobby request message for the server
            byte[] buffer = Encoding.UTF8.GetBytes($"join-lobby:{lobbyId}");

            // Send the request to the server via the network stream
            await stream.WriteAsync(buffer);

            // After sending the join request, wait for the server's "start-game" signal
            await WaitForGameStart();
        }

        /// <summary>
        /// Waits for the server to send the game start signal
        /// </summary>
        private async Task WaitForGameStart()
        {
            // Buffer to store incoming server messages
            byte[] buffer = new byte[10000];

            // Continuously read messages from the server while connected
            while (tcpClient.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break; // Break if there’s no data

                // Decode the message from the server
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received message from server: {message}");

                // Check the server message type and respond accordingly
                if (message == "lobby-joined")
                {
                    // Server confirmed the player has joined the lobby
                    Console.WriteLine("Joined lobby successfully. Waiting for other player...");
                    this.IsEnabled = false; // Disable window to prevent joining another lobby while waiting for pokemon to load in
                    this.loadingWindow.Show(); // Show the waiting window (embedded in class so that we cannot lose this instance)
                }
                else if (message == "lobby-full")
                {
                    // The lobby is full, waiting for the game to begin
                    Console.WriteLine("Lobby is full. Waiting for the game to start...");
                }
                else if (message.StartsWith("start-game"))
                {
                    // The server sends "start-game" when the game is ready to begin
                    // Determine if this player is Player 1 or Player 2 based on the message content
                    bool isPlayerOne = message == "start-game:player1";

                    // Invoke UI actions on the main thread to start the game window
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        // Create and show the LobbyWindow for the game
                        var gameWindow = new LobbyWindow(tcpClient, isPlayerOne);
                        var loadingPokemonWindow = new LoadingWindow("Waiting for pokemon to load in.");
                        await ShowWaitingWindowTime(loadingPokemonWindow, gameWindow, isPlayerOne, 25000); // 24 pokemon * 1100ms

                        // Close the current SelectLobbyWindow
                        this.Close();
                    });
                    break; // Exit loop once game starts
                }
            }
            // Console log when the connection is closed or no more messages from server
            Console.WriteLine("Connection closed or no more messages from server.");
            this.loadingWindow.Close(); // Close loading window (Because the lobby is full))
        }

        private async Task ShowWaitingWindowTime(LoadingWindow loadingWindow, LobbyWindow gameWindow, bool isPlayerOne, int timeInMilliSeconds)
        {
            loadingWindow.Show(); // Show the waiting window
            await Task.Delay(timeInMilliSeconds); // Wait X time
            // After waiting, close loading window and show the game window
            loadingWindow.Close();
            gameWindow.Show();
        }
    }
}
