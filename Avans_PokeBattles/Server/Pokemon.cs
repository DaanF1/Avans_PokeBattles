using Avans_PokeBattles.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles
{
    public class Pokemon
    {
        // Attributes
        private string pokemonName;
        private Uri previewUri;
        private Uri battleForUri;
        private Uri battleAgainstUri;
        private List<Move> pokemonMoves = new List<Move>();
        private int maxHealth;
        private int currentHealth;
        private int speed;

        public Pokemon() { }
        public Pokemon(string pokemonName, Uri previewUri, Uri battleForUri, Uri battleAgainstUri, List<Move> moves, int maxHealth, int speed)
        {
            this.pokemonName = pokemonName;
            this.previewUri = previewUri;
            this.battleForUri = battleForUri;
            this.battleAgainstUri = battleAgainstUri;
            pokemonMoves = moves;
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            this.speed = speed;
        }

        public string Name { get { return pokemonName; } }

        // Pokemons static image:
        public Uri PreviewUri { get { return previewUri; } }

        // Pokemons backside gif:
        public Uri BattleForUri { get { return battleForUri; } }

        // Pokemons frontside gif:
        public Uri BattleAgainstUri { get { return battleAgainstUri; } }

        public int Health { get { return maxHealth; } }

        public int Speed { get { return speed; } }

        /// <summary>
        /// Gets a move based on the position of the move in the list
        /// </summary>
        /// <param name="position"></param>
        public Move GetMove(int position)
        {
            if (position >= 0 && position < pokemonMoves.Count)
                return pokemonMoves[position];
            return null;
        }

        /// <summary>
        /// Gets a random Move of the Pokemon
        /// </summary>
        public Move GetRandomMove()
        {
            Random random = new Random();
            return pokemonMoves[random.Next(pokemonMoves.Count)];
        }

    }
}
