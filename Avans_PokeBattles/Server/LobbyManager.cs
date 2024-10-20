using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Avans_PokeBattles.Server
{
    public class LobbyManager
    {
        private List<Lobby> lobbies = new List<Lobby>();

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
            List<string> availableLobbies = new List<string>();
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
        public bool TryJoinLobby(string lobbyId, TcpClient client)
        {
            var lobby = lobbies.Find(l => l.LobbyId == lobbyId);
            if (lobby != null && !lobby.IsFull)
            {
                lobby.AddPlayer(client);
                return true;
            }

            return false;
        }

        // Check if a specific lobby is full
        public bool IsLobbyFull(int lobbyNumber)
        {
            return lobbies[lobbyNumber-1].IsFull;
        }
    }
}
