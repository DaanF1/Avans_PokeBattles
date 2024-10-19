using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Client
{
    internal class Player
    {
        private string name;
        private List<Pokemon> pokemon;

        public Player(string name, List<Pokemon> pokemon)
        {
            this.name = name;
            this.pokemon = pokemon;
        }

        public string PlayerName { get { return this.name; } }

        public Pokemon GetPlayerPokemon()
        {
            for (int i = 0; i < this.pokemon.Count; i++)
            {
                if (this.pokemon[i].Health > 0)
                {
                    return this.pokemon[i];
                }
            }
            return null;
        }

        public Pokemon GetPokemon(int position)
        {
            if (position >= 0 && position < this.pokemon.Count)
                return this.pokemon[position];
            return null;
        }

    }
}
