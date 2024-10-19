using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Avans_PokeBattles.Client
{
    /// <summary>
    /// Interaction logic for LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        // Public PokemonLister
        private PokemonLister lister = new PokemonLister();

        // Uri prefixes for loading images
        public string uriPrefix = System.AppDomain.CurrentDomain.BaseDirectory;
        public UriKind standardUriKind = UriKind.Absolute;

        // Pokemon Battle boxes
        public MediaState PPlayer1State;
        public MediaState PPlayer2State;

        public LobbyWindow(string playerName)
        {
            InitializeComponent();

            // Set name
            lblPlayer1Name.Content = playerName;

            // Create Pokemon & Moves
            List<Move> unownMoves = new List<Move>();
            unownMoves.Add(new Move("Hidden Power", 60, 100, Type.Normal));
            unownMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            unownMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            unownMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            Pokemon unown = new Pokemon("Unown", new Uri(uriPrefix + "/Sprites/aUnownPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aUnownFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aUnownAgainst.gif", standardUriKind), unownMoves, 90);

            List<Move> venusaurMoves = new List<Move>();
            venusaurMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            venusaurMoves.Add(new Move("Take Down", 90, 85, Type.Normal));
            venusaurMoves.Add(new Move("Razor Leaf", 55, 95, Type.Grass));
            venusaurMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(uriPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), venusaurMoves, 70);

            List<Move> charizardMoves = new List<Move>();
            charizardMoves.Add(new Move("Scratch", 40, 100, Type.Normal));
            charizardMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            charizardMoves.Add(new Move("Slash", 70, 100, Type.Normal));
            charizardMoves.Add(new Move("Flamethrower", 90, 100, Type.Fire));
            Pokemon charizard = new Pokemon("Charizard", new Uri(uriPrefix + "/Sprites/aCharizardPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aCharizardFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aCharizardAgainst.gif", standardUriKind), charizardMoves, 120);

            List<Move> blastoiseMoves = new List<Move>();
            blastoiseMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            blastoiseMoves.Add(new Move("Aqua Tail", 90, 90, Type.Water));
            blastoiseMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            blastoiseMoves.Add(new Move("Rapid Spin", 50, 100, Type.Normal));
            Pokemon blastoise = new Pokemon("Blastoise", new Uri(uriPrefix + "/Sprites/aBlastoisePreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aBlastoiseFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aBlastoiseAgainst.gif", standardUriKind), blastoiseMoves, 75);

            // Create Pokemon and add them to the lister
            lister.AddPokemon(unown);
            lister.AddPokemon(venusaur);
            lister.AddPokemon(charizard);
            lister.AddPokemon(blastoise);
        }

        /// <summary>
        /// Generated methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set Random Pokemon Player 1
            Pokemon p1P1 = lister.GetRandomPokemon();
            Pokemon p1P2 = lister.GetRandomPokemon();
            Pokemon p1P3 = lister.GetRandomPokemon();
            Pokemon p1P4 = lister.GetRandomPokemon();
            Pokemon p1P5 = lister.GetRandomPokemon();
            Pokemon p1P6 = lister.GetRandomPokemon();

            // Set Preview Pokemon Player 1
            P1Pokemon1Preview.Source = new BitmapImage(lister.GetPokemon(p1P1.Name).PreviewUri);
            P1Pokemon2Preview.Source = new BitmapImage(lister.GetPokemon(p1P2.Name).PreviewUri);
            P1Pokemon3Preview.Source = new BitmapImage(lister.GetPokemon(p1P3.Name).PreviewUri);
            P1Pokemon4Preview.Source = new BitmapImage(lister.GetPokemon(p1P4.Name).PreviewUri);
            P1Pokemon5Preview.Source = new BitmapImage(lister.GetPokemon(p1P5.Name).PreviewUri);
            P1Pokemon6Preview.Source = new BitmapImage(lister.GetPokemon(p1P6.Name).PreviewUri);

            // Set Random Pokemon Player 1
            Pokemon p2P1 = lister.GetRandomPokemon();
            Pokemon p2P2 = lister.GetRandomPokemon();
            Pokemon p2P3 = lister.GetRandomPokemon();
            Pokemon p2P4 = lister.GetRandomPokemon();
            Pokemon p2P5 = lister.GetRandomPokemon();
            Pokemon p2P6 = lister.GetRandomPokemon();

            // Set Preview Pokemon Player 2
            P2Pokemon1Preview.Source = new BitmapImage(lister.GetPokemon(p2P1.Name).PreviewUri);
            P2Pokemon2Preview.Source = new BitmapImage(lister.GetPokemon(p2P2.Name).PreviewUri);
            P2Pokemon3Preview.Source = new BitmapImage(lister.GetPokemon(p2P3.Name).PreviewUri);
            P2Pokemon4Preview.Source = new BitmapImage(lister.GetPokemon(p2P4.Name).PreviewUri);
            P2Pokemon5Preview.Source = new BitmapImage(lister.GetPokemon(p2P5.Name).PreviewUri);
            P2Pokemon6Preview.Source = new BitmapImage(lister.GetPokemon(p2P6.Name).PreviewUri);

            // Set Battle Pokemon
            PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer1Pokemon(lister.GetPokemon(p1P1.Name).BattleForUri);
            PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer2Pokemon(lister.GetPokemon(p2P1.Name).BattleAgainstUri);
        }

        private void PP1_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Replay gif animation
            PPlayer1State = MediaState.Stop;
            PokemonPlayer1.Position = new TimeSpan(0, 0, 1);
            PokemonPlayer1.Play();
        }

        private void PP1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("Could not load in .gif file!");
        }

        private void PP2_MediaEndend(object sender, RoutedEventArgs e)
        {
            // Replay gif animation
            PPlayer2State = MediaState.Stop;
            PokemonPlayer2.Position = new TimeSpan(0, 0, 1);
            PokemonPlayer2.Play();
        }

        private void PP2_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("Could not load in .gif file!");
        }

        private void btnSendChat_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void btnOption1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOption2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOption3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOption4_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Self-made methods
        /// </summary>
        private void SetPlayer1Pokemon(Uri pokemonUri)
        {
            // Set MediaElement to gif
            PokemonPlayer1.Source = pokemonUri;
            PokemonPlayer1.Play();
            Task.Run(() => { RefreshMedia1Element(); }); // Start refreshing the MediaElement
        }

        private void SetPlayer2Pokemon(Uri pokemonUri)
        {
            // Set MediaElement to gif
            PokemonPlayer2.Source = pokemonUri;
            PokemonPlayer2.Play();
            Task.Run(() => { RefreshMedia2Element(); }); // Start refreshing the MediaElement
        }

        private void RefreshMedia1Element()
        {
            // Update event for MediaElement of Player 1
            while (PPlayer1State == MediaState.Manual || PPlayer1State == MediaState.Play)
            {
                PPlayer1State = GetMediaState(PokemonPlayer1);

                // TODO: Prevent gif from leaving dots around
                //cvGameView.BringIntoView();
            }
        }

        private void RefreshMedia2Element()
        {
            // Update event for MediaElement of Player 2
            while (PPlayer2State == MediaState.Manual || PPlayer2State == MediaState.Play)
            {
                PPlayer2State = GetMediaState(PokemonPlayer2);

                // TODO: Prevent gif from leaving dots around
                //cvGameView.BringIntoView();
            }
        }

        /// <summary>
        /// Helper method to get the state of the MediaElement
        /// From StackOverflow: https://stackoverflow.com/questions/4338951/how-do-i-determine-if-mediaelement-is-playing
        /// </summary>
        /// <param name="myMedia"></param>
        /// <returns></returns>
        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo? hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object? helperObject = hlp.GetValue(myMedia);
            FieldInfo? stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState? state = (MediaState)stateField.GetValue(helperObject);
            if (!state.Equals(null))
                return (MediaState)state;
            return MediaState.Stop;
        }

    }
}