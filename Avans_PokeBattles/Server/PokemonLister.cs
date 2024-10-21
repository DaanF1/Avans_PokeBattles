using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Avans_PokeBattles.Server
{
    internal class PokemonLister
    {
        // List with all availible pokemon
        private List<Pokemon> pokemonList = new List<Pokemon>();

        public PokemonLister() { }

        public void AddPokemon(Pokemon newPokemon)
        {
            // Add new Pokemon to the list
            if (!pokemonList.Contains(newPokemon))
                pokemonList.Add(newPokemon);
        }

        /// <summary>
        /// Gets the Pokemon based on its Name
        /// If it isn't found we return Unown
        /// </summary>
        /// <param name="pokemonName"></param>
        public Pokemon GetPokemon(string pokemonName)
        {
            Pokemon pokemonToGet = pokemonList.FirstOrDefault(pokemon => pokemon.Name == pokemonName);
            if (pokemonToGet != null)
            {
                return pokemonToGet;
            }
            return GetPokemon("Unown");
        }

        /// <summary>
        /// Gets a random Pokemon from the list
        /// </summary>
        public Pokemon GetRandomPokemon()
        {
            Random random = new Random();
            Pokemon randomPokemon = pokemonList[random.Next(pokemonList.Count)];
            if (randomPokemon.Name == "Unown")
                return GetRandomPokemon();
            return randomPokemon;
        }

    }
}
