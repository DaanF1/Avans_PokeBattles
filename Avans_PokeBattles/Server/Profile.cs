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
        private List<Pokemon> Team { get; set; }
        private int Wins { get; set; }
        private int Losses { get; set; }

        public Profile(string name) 
        { 
            this.Name = name;
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
            if (this.Team.Count >= 6)
                this.Team.Add(pokemon);
        }

        public void RemovePokemonFromTeam(Pokemon pokemon)
        {
            if (this.Team.Contains(pokemon))
                this.Team.Remove(pokemon);
        }

        public void RemoveTeam()
        {
            this.Team.ForEach(pokemon => { this.RemovePokemonFromTeam(pokemon); });
        }

        public string GetName()
        {
            return this.Name;
        }

        public List<Pokemon> GetTeam()
        {
            return this.Team;
        }

    }
}