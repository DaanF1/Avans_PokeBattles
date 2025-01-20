using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Server
{
    public class ProfileManager
    {
        private static readonly ProfileManager _instance = new ProfileManager();
        private readonly Dictionary<string, Profile> profiles;

        public ProfileManager()
        {
            profiles = new Dictionary<string, Profile>();
            LoadProfiles();
        }

        public static ProfileManager Instance => _instance;

        public Profile CreateProfile(string playerName, TcpClient client)
        {
            if (!profiles.ContainsKey(playerName))
            {
                var profile = new Profile(playerName, client);
                profiles[playerName] = profile;
                SaveProfile(profile);
            }
            else
            {
                profiles[playerName].SetTcpClient(client);
            }
            return profiles[playerName];
        }

        public Profile GetProfile(string playerName)
        {
            return profiles.ContainsKey(playerName) ? profiles[playerName] : null;
        }

        public Profile GetProfileViaTcpClient(TcpClient client)
        {
            EndPoint endPoint = client.Client.RemoteEndPoint;
            foreach (Profile getProfile in profiles.Values)
            {
                TcpClient tcpClient = getProfile.GetTcpClient();
                if (tcpClient != null)
                {
                    EndPoint local = tcpClient.Client.LocalEndPoint;
                    string[] list = local.ToString().Split(":");
                    string localEndPoint = list[3].Replace("]", ":") + list[4];
                    // Address:Portnumber
                    if (endPoint != null && localEndPoint == endPoint.ToString())
                        return getProfile;
                }
            }
            return null;
        }

        public void LoadProfiles()
        {
            using var connection = Database.GetConnection();
            connection.Open();

            string query = "SELECT * FROM Profiles";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(0);
                int wins = reader.GetInt32(1);
                int losses = reader.GetInt32(2);

                var profile = new Profile(name, null)
                {
                    Wins = wins,
                    Losses = losses,
                };
                profiles[name] = profile;
            }
        }

        public void SaveProfile(Profile profileToSave)
        {
            using var connection = Database.GetConnection();
            connection.Open();

            string query = "INSERT INTO Profiles (Name, Wins, Losses) VALUES (@Name, @Wins, @Losses) " +
                           "ON CONFLICT(Name) DO UPDATE SET Wins = excluded.Wins, Losses = excluded.Losses";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Name", profileToSave.GetName());
            command.Parameters.AddWithValue("@Wins", profileToSave.GetWins());
            command.Parameters.AddWithValue("@Losses", profileToSave.GetLosses());

            using var reader = command.ExecuteReader();
        }
    }
}
