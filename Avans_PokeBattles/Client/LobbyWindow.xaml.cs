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
        public MediaState PPlayer1State;
        public MediaState PPlayer2State;
        public LobbyWindow(string playerName)
        {
            InitializeComponent();
            // Set names
            lblPlayer1Name.Content = playerName;
        }

        /// <summary>
        /// Generated methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Create Pokemon
            Pokemon mVenusaurAgainst = new Pokemon(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurAgainst.gif", UriKind.Absolute));
            Pokemon mVenusaurRegular = new Pokemon(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurRegular.gif", UriKind.Absolute));

            // Set Preview Pokemon
            P1Pokemon1Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P1Pokemon2Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P1Pokemon3Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P1Pokemon4Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P1Pokemon5Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P1Pokemon6Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));

            P2Pokemon1Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P2Pokemon2Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P2Pokemon3Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P2Pokemon4Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P2Pokemon5Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));
            P2Pokemon6Preview.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites/mVenusaurPreview.png", UriKind.Absolute));

            // Set Battle Pokemon
            PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer1Pokemon(mVenusaurRegular.GetUri());
            PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer2Pokemon(mVenusaurAgainst.GetUri());
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
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }


    }
}
