using Avans_PokeBattles.Client;
using System;
using System.Net.Sockets;

namespace Avans_PokeBattles.Server
{
    public class Lobby
    {
        private TcpClient player1;
        private TcpClient player2;
        private NetworkStream stream1;
        private NetworkStream stream2;

        public string LobbyId { get; private set; }
        public bool IsFull { get; private set; }

        public Lobby(string lobbyId)
        {
            LobbyId = lobbyId;
            IsFull = false;
        }

        public void AddPlayer(TcpClient client)
        {
            if (player1 == null)
            {
                player1 = client;
                stream1 = client.GetStream();
            }
            else if (player2 == null)
            {
                player2 = client;
                stream2 = client.GetStream();
                IsFull = true;
                StartGame();
            }
        }

        private async void StartGame()
        {
            // Start handling communication between the two players
            Task.Run(() => HandleClient(player1, stream1, player2, stream2));
            Task.Run(() => HandleClient(player2, stream2, player1, stream1));
        }

        private async Task HandleClient(TcpClient sender, NetworkStream senderStream, TcpClient receiver, NetworkStream receiverStream)
        {
            byte[] buffer = new byte[1500];
            while (sender.Connected && receiver.Connected)
            {
                int bytesRead = await senderStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                // Relay message to the other player
                await receiverStream.WriteAsync(buffer, 0, bytesRead);
            }

            // Handle disconnection
            sender.Close();
            receiver.Close();
        }
    }
}
