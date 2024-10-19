using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Avans_PokeBattles.Client
{
    class PokemonLister
    {
        // List with all availible pokemon
        private List<Pokemon> pokemonList = new List<Pokemon>();

        public PokemonLister() { }

        public void AddPokemon(Pokemon newPokemon)
        {
            pokemonList.Add(newPokemon);
        }

        public Pokemon GetPokemon(string pokemonName)
        {
            Pokemon pokemonToGet = pokemonList.FirstOrDefault(pokemon => pokemon.Name == pokemonName);
            if (pokemonToGet != null)
            {
                return pokemonToGet;
            }
            return GetPokemon("Unown");
        }

        //public void RemovePokemon(string pokemonName) 
        //{
        //    Pokemon pokemonToRemove = pokemonList.FirstOrDefault(pokemon => pokemon.GetName() == pokemonName);
        //    if (pokemonToRemove != null)
        //    {
        //        pokemonList.Remove(pokemonToRemove);
        //    }
        //}

    }
}
