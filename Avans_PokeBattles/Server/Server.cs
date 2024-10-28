using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Avans_PokeBattles.Server
{
    public class Server
    {
        private static TcpListener listener;
        private static readonly int port = 8000;
        private static LobbyManager lobbyManager = new();
        private static Lobby connectedLobby;
        private static readonly Dictionary<TcpClient, string> clientNames = [];
        private static bool isRunning = false;

        public static async void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;
            Console.WriteLine("SERVER: Server started. Waiting for connections...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("SERVER: Client connected.");
                Task.Run(() => HandleClientAsync(client));
            }
        }

        public static void Stop()
        {
            isRunning = false;
            Stop(); // Stop the Server
        }

        public static bool IsRunning()
        {
            return isRunning;
        }

        // Handle individual client requests
        private static async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[10000];
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"SERVER: Received message: {message}");

                // Handle getting names
                if (message.StartsWith("Player name:"))
                {
                    string name = message.Split(':')[1].Trim();
                    clientNames.Add(client, name);
                }
                // Handle join-lobby request
                if (message.StartsWith("join-lobby"))
                {
                    string[] splitMessage = message.Split(':');
                    if (splitMessage.Length == 2)
                    {
                        // Get current Player name
                        string namePlayer1 = "";
                        int index = 0;
                        foreach (TcpClient tcp in clientNames.Keys)
                        {
                            if (tcp == client)
                                namePlayer1 = clientNames.Values.ElementAt(index);
                            index++;
                        }

                        string lobbyId = splitMessage[1];
                        bool joined = lobbyManager.TryJoinLobby(lobbyId, client, namePlayer1);

                        // Notify the client if joining the lobby was successful
                        string responseMessage = joined ? "lobby-joined" : "lobby-full";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await stream.WriteAsync(response, 0, response.Length);

                        // If the lobby is now full, start the game
                        if (joined && lobbyManager.GetCurrentLobby(int.Parse(lobbyId.Split('-')[1])).IsFull)
                        {
                            connectedLobby = lobbyManager.GetCurrentLobby(int.Parse(lobbyId.Split('-')[1]));
                            Console.WriteLine("SERVER: Lobby is full, starting the game...");
                            connectedLobby.StartGame(); // Only start the game when lobby is confirmed full
                        }
                    }
                }
                else if (message.StartsWith("move:"))
                {
                    // Handle move command by forwarding it to the appropriate lobby
                    var lobby = lobbyManager.GetLobbyForClient(client);
                    if (lobby != null)
                    {
                        Console.WriteLine($"SERVER: Forwarding move to Lobby {lobby.LobbyId}");
                        await lobby.HandleMove(message, client);
                    }
                    else
                    {
                        Console.WriteLine("SERVER: Client not assigned to any lobby.");
                    }
                }
                else if (message.StartsWith("chat:")) 
                {
                    var lobby = lobbyManager.GetLobbyForClient(client);
                    if (lobby != null)
                    {
                        Console.WriteLine($"SERVER: Forwarding message to Lobby {lobby.LobbyId}");
                        await lobby.HandleChat(message, client);
                    }
                    else
                    {
                        Console.WriteLine("SERVER: Client not assigned to any lobby.");
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