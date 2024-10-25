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
        private static LobbyManager lobbyManager = new LobbyManager();

        public async void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                Task.Run(() => HandleClientAsync(client));
            }
        }

        // Handle individual client requests
        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[10000];
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received message: {message}");

                // Handle client requests
                if (message == "list-lobbies")
                {
                    // Send the list of available lobbies to the client
                    string lobbyList = lobbyManager.GetLobbyList();
                    byte[] response = Encoding.UTF8.GetBytes(lobbyList);
                    await stream.WriteAsync(response, 0, response.Length);
                }
                else if (message.StartsWith("join-lobby"))
                {
                    // Parse the lobby ID from the message
                    string[] splitMessage = message.Split(':');
                    if (splitMessage.Length == 2)
                    {
                        string lobbyId = splitMessage[1];
                        bool joined = lobbyManager.TryJoinLobby(lobbyId, client);

                        // Notify the client if joining the lobby was successful
                        string responseMessage = joined ? "lobby-joined" : "lobby-full";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await stream.WriteAsync(response, 0, response.Length);

                        // If lobby is now full, start game and send teams
                        if (joined && lobbyManager.GetCurrentLobby(int.Parse(lobbyId.Split('-')[1])).IsFull)
                        {
                            var lobby = lobbyManager.GetCurrentLobby(int.Parse(lobbyId.Split('-')[1]));
                            lobby.StartGame();
                        }
                    }
                }
            }

            client.Close();
        }

        // Get the lobby manager instance
        internal static LobbyManager GetLobbymanager()
        {
            return lobbyManager;
        }
    }
}