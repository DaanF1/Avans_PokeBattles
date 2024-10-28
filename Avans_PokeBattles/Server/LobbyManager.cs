using System.Net.Sockets;

namespace Avans_PokeBattles.Server
{
    public class LobbyManager
    {
        private readonly List<Lobby> lobbies = [];

        public LobbyManager()
        {
            // Initialize with some default lobbies
            lobbies.Add(new Lobby("Lobby-1"));
            lobbies.Add(new Lobby("Lobby-2"));
            lobbies.Add(new Lobby("Lobby-3"));
        }

        // Provide the client with a list of available lobbies
        public string GetLobbyList()
        {
            List<string> availableLobbies = [];
            foreach (var lobby in lobbies)
            {
                if (!lobby.IsFull)
                {
                    availableLobbies.Add(lobby.LobbyId);
                }
            }

            // Return as a comma-separated string
            return string.Join(",", availableLobbies);
        }

        // Allow the client to join a specific lobby by its ID
        public bool TryJoinLobby(string lobbyId, TcpClient client, string playerName)
        {
            var lobby = lobbies.Find(l => l.LobbyId == lobbyId);
            if (lobby != null && !lobby.IsFull)
            {
                lobby.AddPlayer(client, playerName);
                return true;
            }

            return false;
        }

        // Check if a specific lobby is full
        public bool IsLobbyFull(int lobbyNumber)
        {
            return lobbies[lobbyNumber - 1].IsFull;
        }

        // Get current Lobby
        public Lobby GetCurrentLobby(int lobbyNumber)
        {
            return lobbies[lobbyNumber - 1];
        }

        public Lobby GetLobbyForClient(TcpClient client)
        {
            return lobbies.FirstOrDefault(lobby => lobby.HasClient(client));
        }

    }
}