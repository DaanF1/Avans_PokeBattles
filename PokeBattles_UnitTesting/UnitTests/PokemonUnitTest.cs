using Avans_PokeBattles;
using Avans_PokeBattles.Server;
using Type = Avans_PokeBattles.Server.Type;

namespace PokeBattles_UnitTesting.UnitTests
{
    [TestClass]
    public class PokemonUnitTest
    {
        public string dirPrefix = System.AppDomain.CurrentDomain.BaseDirectory; // Directory prefix for files
        public UriKind standardUriKind = UriKind.Absolute; // Always get the absolute path

        [TestMethod]
        public void PokemonListerTest() // Unit Test: Adding and getting a Pokemon from the PokemonLister
        {
            // Create PokemonLister
            PokemonLister pokemonLister = new Avans_PokeBattles.Server.PokemonLister();
            pokemonLister = new Avans_PokeBattles.Server.PokemonLister();
            // Create Pokemon
            List<Move> venusaurMoves = new List<Move>();
            venusaurMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            venusaurMoves.Add(new Move("Take Down", 90, 85, Type.Normal));
            venusaurMoves.Add(new Move("Razor Leaf", 55, 95, Type.Grass));
            venusaurMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), Type.Grass, 195, 70, venusaurMoves);
            // Add Pokemon to PokemonLister
            pokemonLister.AddPokemon(venusaur);

            // Get Pokemon from PokemonLister (Name)
            Pokemon gottenPokemon = pokemonLister.GetPokemon(venusaur.Name);
            if (gottenPokemon.Name != "Venusaur")
                Assert.IsTrue(false); // Error

            // Get Pokemon from PokemonLister (Random)
            Pokemon anotherPokemon = pokemonLister.GetRandomPokemon();
            if (anotherPokemon.Name != "Venusaur")
                Assert.IsTrue(false); // Error

            // Test is successful
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PokemonMoveTest() // Unit Test: Getting Moves from a Pokemon
        {
            // Create PokemonLister
            PokemonLister pokemonLister = new Avans_PokeBattles.Server.PokemonLister();
            // Create Pokemon
            List<Move> venusaurMoves = new List<Move>();
            venusaurMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            venusaurMoves.Add(new Move("Take Down", 90, 85, Type.Normal));
            venusaurMoves.Add(new Move("Razor Leaf", 55, 95, Type.Grass));
            venusaurMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), Type.Grass, 195, 70, venusaurMoves);
            // Add Pokemon to PokemonLister
            pokemonLister.AddPokemon(venusaur);
            // Get Pokemon from PokemonLister
            Pokemon pokemon = pokemonLister.GetPokemon("Venusaur");

            // Random loop numbers
            for (int i = -5; i < 9; i++)
            {
                Move move = pokemon.GetMove(i);
                if (move == null) // Move should be null if it isn't in the scope (0-4)
                    continue;

                // Extra checks
                if (!move.Equals(null) && i < 0)
                    Assert.IsTrue(false); // Error
                if (move.Equals(null) && i >= 0 && i < 5)
                    Assert.IsTrue(false); // Error
                if (!move.Equals(null) && i >= 5)
                    Assert.IsTrue(false); // Error
            }

            // Test is successful
            Assert.IsTrue(true);
        }
    }
}