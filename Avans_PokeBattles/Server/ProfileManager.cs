using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        }

        public static ProfileManager Instance => _instance;

        public Profile GetOrCreateProfile(string playerName, TcpClient client)
        {
            if (!profiles.ContainsKey(playerName))
            {
                profiles[playerName] = new Profile(playerName, client);
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
                TcpClient tcpClient = getProfile.GetTcpCLient();
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
    }
}
