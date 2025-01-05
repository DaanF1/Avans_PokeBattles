using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Server
{
    public class Profile
    {
        private string Name { get; set; }
        private TcpClient TcpClient { get; set; }
        private List<Pokemon> Team { get; set; }
        private int Wins { get; set; }
        private int Losses { get; set; }

        public Profile(string name, TcpClient client) 
        { 
            this.Name = name;
            this.TcpClient = client;
            this.Team = new List<Pokemon>();
            this.Wins = 0;
            this.Losses = 0;
        }

        public void IncrementWins() 
        {
            Wins++;
        }

        public void IncrementLosses() 
        { 
            Losses++; 
        }

        public void AddPokemonToTeam(Pokemon pokemon)
        {
            if (this.Team.Count <= 6)
                this.Team.Add(pokemon.DeepCopy());
        }

        public void RemoveTeam()
        {
            if (this.Team.Count > 0)
            {
                this.Team = new List<Pokemon>();
            }
        }

        public string GetName()
        {
            return this.Name;
        }

        public TcpClient GetTcpClient()
        {
            return this.TcpClient;
        }

        public List<Pokemon> GetTeam()
        {
            return this.Team;
        }

        public int GetWins()
        {
            return this.Wins;
        }

        public int GetLosses()
        {
            return this.Losses;
        }

    }
}