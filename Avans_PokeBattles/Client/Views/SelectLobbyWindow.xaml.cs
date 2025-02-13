﻿using Avans_PokeBattles.Client.Views;
using Avans_PokeBattles.Server;
using System.Diagnostics.Eventing.Reader;
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
        private readonly Profile playerProfile;
        private readonly int port = 8000;
        public LoadingWindow loadingWindow = new LoadingWindow("Waiting for another player."); // Make loading window accessable

        public SelectLobbyWindow(Profile profile)
        {
            InitializeComponent();

            // Set up the TCP client and network stream with the provided client
            this.tcpClient = profile.GetTcpClient();
            this.stream = profile.GetTcpClient().GetStream();

            // Retrieve the lobby manager instance from the server
            this.lobbyManager = Server.Server.GetLobbymanager();

            // Set the player’s name and update the label with this information
            this.playerProfile = profile;
            lblName.Content = $"Name: {profile.GetName()}";

            // Set Wins & Losses
            if (profile.GetWins() > 999999999)
                lblWins.Content = $"Wins: 999999999";
            else
                lblWins.Content = $"Wins: {profile.GetWins()}";

            if (profile.GetLosses() > 999999999)
                lblLosses.Content = $"Losses: 999999999";
            else
                lblLosses.Content = $"Losses: {profile.GetLosses()}";

            listTeam.Items.Clear(); // Clear the list to fill with new Team
            listTeam.DisplayMemberPath = "Name"; // Display Name of Pokemon in list
            
            // Only allow team to be set once
            if (profile.GetTeam().Count == 6)
                btnTeam.IsEnabled = false;

            // Re-add Pokemon to list
            if (profile.GetTeam().Count > 0)
            {
                profile.GetTeam().ForEach(p=>listTeam.Items.Add(p));
            }
        }

        // Event handler triggered when the SelectLobbyWindow is loaded
        private void SelectLobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Request the list of available lobbies from the server
            lobbyManager.GetLobbyList();
        }

        // Asynchronous event handlers for the Join Lobby buttons
        // Each button calls the JoinLobby method with a specific lobby ID and number
        private async void btnJoinLobby1_Click(object sender, RoutedEventArgs e)
        {
            if (playerProfile.GetTeam().Count != 6)
            {
                MessageBox.Show("Cannot join a Lobby without a full team!", "Invalid Join", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            await JoinLobby("Lobby-1", 1);
        }
        private async void btnJoinLobby2_Click(object sender, RoutedEventArgs e)
        {
            if (playerProfile.GetTeam().Count != 6)
            {
                MessageBox.Show("Cannot join a Lobby without a full team!", "Invalid Join", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            await JoinLobby("Lobby-2", 2);
        }

        private async void btnJoinLobby3_Click(object sender, RoutedEventArgs e)
        {
            if (playerProfile.GetTeam().Count != 6)
            {
                MessageBox.Show("Cannot join a Lobby without a full team!", "Invalid Join", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            await JoinLobby("Lobby-3", 3);
        }

        private void btnTeam_Click(object sender, RoutedEventArgs e)
        {
            // Go to create team window
            var createTeamWindow = new CreateTeamWindow(playerProfile, Server.Server.GetPokemonLister());
            createTeamWindow.Show();
            this.Hide();
        }

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
            bool isPlayerOne = false;

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
                    isPlayerOne = message == "start-game:player1";

                    this.loadingWindow.Close(); // Close loading window (Because the lobby is full))

                    var gameWindow = new LobbyWindow(playerProfile, isPlayerOne);
                    gameWindow.Show();
                    this.Hide();

                    break; // Exit loop once game starts
                }
            }
            // Console log when the connection is closed or no more messages from server
            Console.WriteLine("Connection closed or no more messages from server.");
        }

    }
}
