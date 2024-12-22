using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Server
{
    public class ProfileManager
    {
        private readonly Dictionary<string, Profile> profiles;

        public ProfileManager()
        {
            profiles = new Dictionary<string, Profile>();
        }

        public Profile GetOrCreateProfile(string playerName)
        {
            if (!profiles.ContainsKey(playerName))
            {
                profiles[playerName] = new Profile(playerName);
            }
            return profiles[playerName];
        }

        public Profile GetProfile(string playerName)
        {
            return profiles.ContainsKey(playerName) ? profiles[playerName] : null;
        }
    }
}
