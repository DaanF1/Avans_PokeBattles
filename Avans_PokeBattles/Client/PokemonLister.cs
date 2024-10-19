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

        public void AddPokemon(string name, Uri previewUri, Uri battleForUri, Uri battleAgainstUri)
        {
            Pokemon newPokemon = new Pokemon(name, previewUri, battleForUri, battleAgainstUri);
            pokemonList.Add(newPokemon);
        }

        public void RemovePokemon(string name) 
        {
            Pokemon pokemonToRemove = pokemonList.FirstOrDefault(pokemon => pokemon.GetName() == name);
            if (pokemonToRemove != null)
            {
                pokemonList.Remove(pokemonToRemove);
            }
        }
        public Pokemon GetPokemon(string pokemonName)
        {
            foreach (Pokemon pokemon in pokemonList)
            {
                if (pokemon.GetName().Equals(pokemonName))
                {
                    return pokemon;
                }
            }
            return null;
        }

    }
}
