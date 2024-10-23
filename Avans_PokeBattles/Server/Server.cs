using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Server
{
    public class Server
    {
        private static TcpListener listener;
        private static int port = 8000;
        private TcpClient player1;
        private TcpClient player2;
        private string player1Name = null;
        private string player2Name = null;

        public async void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                if (player1 == null)
                {
                    player1 = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Player 1 connected.");
                    Task.Run(() => HandleClient(player1, 1));
                }
                else if (player2 == null)
                {
                    player2 = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Player 2 connected.");
                    Task.Run(() => HandleClient(player2, 2));
                }
            }
        }

        private async Task HandleClient(TcpClient client, int playerNumber)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1500];

            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Message from player {playerNumber}: {message}");

                if (message.StartsWith("Player name: "))
                {
                    string playerName = message.Substring("Player name: ".Length);

                    if (playerNumber == 1)
                        player1Name = playerName;
                    else
                        player2Name = playerName;

                    Console.WriteLine($"Player {playerNumber} name set to {playerName}");

                    // If both players have entered their names, notify them to start the game
                    if (player1Name != null && player2Name != null)
                    {
                        Task.Run(() => StartGame());
                    }
                }
            }

            // Handle disconnection
            client.Close();
        }

        private async Task StartGame()
        {
            NetworkStream stream1 = player1.GetStream();
            NetworkStream stream2 = player2.GetStream();

            // Notify both players to open their game window
            await SendMessage(stream1, $"start-game:Player 1:{player1Name},Player 2:{player2Name}");
            await SendMessage(stream2, $"start-game:Player 1:{player1Name},Player 2:{player2Name}");

            // You can now send initial game state (team selection, etc.) to both players here
            Task.Run(() => HandleGameLogic(player1, stream1, player2, stream2));
        }

        private async Task HandleGameLogic(TcpClient player1, NetworkStream stream1, TcpClient player2, NetworkStream stream2)
        {
            byte[] buffer = new byte[1500];

            // Game logic handling between player 1 and player 2
            // Process moves, game state, etc.
            while (player1.Connected && player2.Connected)
            {
                // Example logic for move handling can be inserted here
            }

            player1.Close();
            player2.Close();
        }

        private async Task SendMessage(NetworkStream stream, string message)
        {
            byte[] response = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(response, 0, response.Length);
            await stream.FlushAsync();
        }
    }
}
