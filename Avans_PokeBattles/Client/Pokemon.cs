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

        public Pokemon(string pokemonName, Uri previewUri, Uri battleForUri, Uri battleAgainstUri)
        {
            this.pokemonName = pokemonName;
            this.previewUri = previewUri;
            this.battleForUri = battleForUri;
            this.battleAgainstUri = battleAgainstUri;
        }

        public string GetName()
        {
            return this.pokemonName;
        }

        public Uri GetPreviewUri()
        {
            return this.previewUri;
        }

        public Uri GetBattleForUri()
        {
            return this.battleForUri;
        }

        public Uri GetBattleAgainstUri()
        {
            return this.battleAgainstUri;
        }
    }
}
