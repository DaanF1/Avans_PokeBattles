namespace Avans_PokeBattles.Server
{
    public class PokemonLister
    {
        // List with all availible pokemon
        private readonly List<Pokemon> pokemonList = [];

        public PokemonLister() { }

        public void AddPokemon(Pokemon newPokemon)
        {
            // Add new Pokemon to the list
            if (!pokemonList.Contains(newPokemon))
                pokemonList.Add(newPokemon);
        }

        public void AddAllPokemon(List<Pokemon> listPokemon)
        {
            foreach (Pokemon pokemon in listPokemon)
            {
                // Add new Pokemon to the list
                if (!pokemonList.Contains(pokemon))
                    pokemonList.Add(pokemon);
            }
        }

        /// <summary>
        /// Gets a random Pokemon from the list
        /// </summary>
        public List<Pokemon> GetAllPokemon()
        {
            List<Pokemon> availiblePokemon = new List<Pokemon>();
            foreach (Pokemon pokemon in pokemonList)
            { 
                if (pokemon.Name != "Unown")
                    availiblePokemon.Add(pokemon); 
            }
            return availiblePokemon;
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
            Random random = new();
            Pokemon randomPokemon = pokemonList[random.Next(pokemonList.Count)];
            if (randomPokemon.Name == "Unown")
                return GetRandomPokemon();
            return randomPokemon;
        }

    }
}