using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Client
{
    class Pokemon
    {
        private string pokemonName;
        private Uri previewUri;
        private Uri battleForUri;
        private Uri battleAgainstUri;
        private List<Move> pokemonMoves = new List<Move>();
        private int speed;

        public Pokemon(string pokemonName, Uri previewUri, Uri battleForUri, Uri battleAgainstUri, List<Move> moves, int speed)
        {
            this.pokemonName = pokemonName;
            this.previewUri = previewUri;
            this.battleForUri = battleForUri;
            this.battleAgainstUri = battleAgainstUri;
            this.pokemonMoves = moves;
            this.speed = speed;
        }

        public string Name { get { return this.pokemonName; } }
        public Uri PreviewUri { get { return this.previewUri; } }
        public Uri BattleForUri { get { return this.battleForUri; } }
        public Uri BattleAgainstUri { get { return this.battleAgainstUri; } }
        public int Speed { get { return this.speed; } }

        public Move GetMove(string moveName)
        {
            return pokemonMoves.FirstOrDefault(move => move.MoveName == moveName);
        }

        public Move GetRandomMove()
        {
            Random random = new Random();
            return pokemonMoves[random.Next(pokemonMoves.Count)];
        }

    }
}
