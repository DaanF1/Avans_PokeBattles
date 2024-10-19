using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Client
{
    class Pokemon
    {
        private Uri uri;

        public Pokemon(Uri uri)
        {
            this.uri = uri;
        }

        public Uri GetUri()
        {
            return this.uri;
        }
    }
}
