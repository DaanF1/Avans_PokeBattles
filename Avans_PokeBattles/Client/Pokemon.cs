using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Client
{
    internal class Pokemon
    {
        // Attributes
        private string pokemonName;
        private Uri previewUri;
        private Uri battleForUri;
        private Uri battleAgainstUri;
        private List<Move> pokemonMoves = new List<Move>();
        private int maxHealth;
        private int speed;

        public Pokemon(string pokemonName, Uri previewUri, Uri battleForUri, Uri battleAgainstUri, List<Move> moves, int maxHealth, int speed)
        {
            this.pokemonName = pokemonName;
            this.previewUri = previewUri;
            this.battleForUri = battleForUri;
            this.battleAgainstUri = battleAgainstUri;
            this.pokemonMoves = moves;
            this.maxHealth = maxHealth;
            this.speed = speed;
        }

        public string Name { get { return this.pokemonName; } }

        // Pokemons static image:
        public Uri PreviewUri { get { return this.previewUri; } }

        // Pokemons backside gif:
        public Uri BattleForUri { get { return this.battleForUri; } }

        // Pokemons frontside gif:
        public Uri BattleAgainstUri { get { return this.battleAgainstUri; } }

        public int Health { get { return this.maxHealth; } }

        public int Speed { get { return this.speed; } }

        /// <summary>
        /// Gets a move based on the position of the move in the list
        /// </summary>
        /// <param name="position"></param>
        public Move GetMove(int position)
        {
            if (position >= 0 && position < this.pokemonMoves.Count)
                return this.pokemonMoves[position];
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
