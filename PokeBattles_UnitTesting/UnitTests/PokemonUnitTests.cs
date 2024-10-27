using Avans_PokeBattles;
using Avans_PokeBattles.Server;
using PokemonLister = Avans_PokeBattles.Server.PokemonLister;
using Type = Avans_PokeBattles.Server.Type;

namespace PokeBattles_UnitTesting.UnitTests
{
    [TestClass]
    public class PokemonUnitTests // Unit Tests for the PokemonLister, Pokemon and Move classes
    {
        public string dirPrefix = System.AppDomain.CurrentDomain.BaseDirectory; // Directory prefix for files
        public UriKind standardUriKind = UriKind.Absolute; // Always get the absolute path

        [TestMethod]
        public void PokemonListerTest() // Unit Test: Adding and getting a Pokemon from the PokemonLister
        {
            PokemonLister pokemonLister = new PokemonLister(); // Create PokemonLister
            Console.WriteLine("PokemonLister Created!");

            pokemonLister = new PokemonLister();
            List<Move> venusaurMoves =
            [
                new Move("Solar Beam", 120, 100, Type.Grass),
                new Move("Take Down", 90, 85, Type.Normal),
                new Move("Razor Leaf", 55, 95, Type.Grass),
                new Move("Tackle", 40, 100, Type.Normal),
            ];  // Create Pokemon
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), 
                new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), 
                new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), 
                Type.Grass, 195, 70, venusaurMoves);
            Console.WriteLine("Pokemon Created!");

            pokemonLister.AddPokemon(venusaur); // Add Pokemon to PokemonLister
            Console.WriteLine("Pokemon added to PokemonLister!");

            Pokemon gottenPokemon = pokemonLister.GetPokemon(venusaur.Name); // Get Pokemon from PokemonLister (Name)
            Console.WriteLine($"Pokemon received: {gottenPokemon.Name}");

            if (gottenPokemon.Name != "Venusaur")
                Assert.IsTrue(false); // Error

            Pokemon anotherPokemon = pokemonLister.GetRandomPokemon(); // Get Pokemon from PokemonLister (Random)
            Console.WriteLine($"Pokemon received: {anotherPokemon.Name}");
            if (anotherPokemon.Name != "Venusaur")
                Assert.IsTrue(false); // Error

            Assert.IsTrue(true); // Test is successful
        }

        [TestMethod]
        public void PokemonMoveTest() // Unit Test: Getting Moves from a Pokemon
        {
            PokemonLister pokemonLister = new PokemonLister(); // Create PokemonLister
            Console.WriteLine("PokemonLister Created!");

            List<Move> venusaurMoves =
            [
                new Move("Solar Beam", 120, 100, Type.Grass),
                new Move("Take Down", 90, 85, Type.Normal),
                new Move("Razor Leaf", 55, 95, Type.Grass),
                new Move("Tackle", 40, 100, Type.Normal),
            ]; // Create Pokemon
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), 
                new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), 
                new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), 
                Type.Grass, 195, 70, venusaurMoves);
            Console.WriteLine("Pokemon Created!");

            pokemonLister.AddPokemon(venusaur); // Add Pokemon to PokemonLister
            Console.WriteLine("Pokemon added to PokemonLister!");

            Pokemon pokemon = pokemonLister.GetPokemon("Venusaur"); // Get Pokemon from PokemonLister
            Console.WriteLine($"Pokemon received: {pokemon.Name}");

            for (int i = -5; i < 9; i++) // Random loop numbers
            {
                Move move = pokemon.GetMove(i);
                if (move == null) // Move should be null if it isn't in the scope (0-4)
                    continue;

                Console.WriteLine($"Move received: {move.MoveName}");

                if (!move.Equals(null) && i < 0) // Extra checks
                    Assert.IsTrue(false); // Error
                if (move.Equals(null) && i >= 0 && i < 5)
                    Assert.IsTrue(false); // Error
                if (!move.Equals(null) && i >= 5)
                    Assert.IsTrue(false); // Error
            }

            Assert.IsTrue(true); // Test is successful
        }
    }
}