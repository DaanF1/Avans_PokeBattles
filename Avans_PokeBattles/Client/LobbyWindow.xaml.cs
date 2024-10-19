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

        // Pokemon Battle boxes
        public MediaState PPlayer1State;
        public MediaState PPlayer2State;
        public string uriPrefix = System.AppDomain.CurrentDomain.BaseDirectory;
        public UriKind standardUriKind = UriKind.Absolute;

        public LobbyWindow(string playerName)
        {
            InitializeComponent();
            // Set names
            lblPlayer1Name.Content = playerName;

            // Create Pokemon and add them to the lister
            lister.AddPokemon("Unown", new Uri(uriPrefix + "/Sprites/aUnownPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aUnownFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aUnownAgainst.gif", standardUriKind));
            lister.AddPokemon("Venusaur", new Uri(uriPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind));
            lister.AddPokemon("Charizard", new Uri(uriPrefix + "/Sprites/aCharizardPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aCharizardFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aCharizardAgainst.gif", standardUriKind));
            
        }

        /// <summary>
        /// Generated methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set Preview Pokemon Player 1
            P1Pokemon1Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P1Pokemon2Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P1Pokemon3Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P1Pokemon4Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P1Pokemon5Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P1Pokemon6Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());

            // Set Preview Pokemon Player 2
            P2Pokemon1Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P2Pokemon2Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P2Pokemon3Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P2Pokemon4Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P2Pokemon5Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());
            P2Pokemon6Preview.Source = new BitmapImage(lister.GetPokemon("Venusaur").GetPreviewUri());

            // Set Battle Pokemon
            PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer1Pokemon(lister.GetPokemon("Venusaur").GetBattleForUri());
            PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer2Pokemon(lister.GetPokemon("Charizard").GetBattleAgainstUri());
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
            while (PPlayer1State == MediaState.Play)
            {
                PPlayer1State = GetMediaState(PokemonPlayer1);
                
            }
        }

        private void RefreshMedia2Element()
        {
            // Update event for MediaElement of Player 2
            while (PPlayer2State == MediaState.Play)
            {
                PPlayer2State = GetMediaState(PokemonPlayer2);

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