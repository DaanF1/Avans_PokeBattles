using Avans_PokeBattles;
using Avans_PokeBattles.Server;

namespace PokeBattles_UnitTesting.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        public string dirPrefix = System.AppDomain.CurrentDomain.BaseDirectory; // Directory prefix for files
        public UriKind standardUriKind = UriKind.Absolute; // Always get the absolute path
        [TestMethod]
        public void TestMethod1() // Unit Test: 
        {
            PokemonLister pokemonLister = new Avans_PokeBattles.Server.PokemonLister();
            List<Move> unownMoves = new List<Move>();
            unownMoves.Add(new Move("Hidden Power", 60, 100, Avans_PokeBattles.Server.Type.Normal));
            unownMoves.Add(new Move("Hydro Pump", 110, 80, Avans_PokeBattles.Server.Type.Water));
            unownMoves.Add(new Move("Inferno", 100, 50, Avans_PokeBattles.Server.Type.Fire));
            unownMoves.Add(new Move("Solar Beam", 120, 100, Avans_PokeBattles.Server.Type.Grass));
            Pokemon unown = new Pokemon("Unown", new Uri(dirPrefix + "/Sprites/aUnownPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aUnownFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aUnownAgainst.gif", standardUriKind), Avans_PokeBattles.Server.Type.Normal, 80, 90, unownMoves);
            pokemonLister.AddPokemon(unown);

            Pokemon gottenPokemon = pokemonLister.GetPokemon("Unown");
            if (gottenPokemon.Name != "Unown")
                Assert.IsTrue(false);

            Pokemon anotherPokemon = pokemonLister.GetRandomPokemon();
            if (anotherPokemon.Name != "Unown")
                Assert.IsTrue(false);

            Assert.IsTrue(true);
        }
    }
}