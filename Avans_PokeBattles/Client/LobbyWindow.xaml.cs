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
        public LobbyWindow()
        {
            InitializeComponent();
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

            // Set Pokemon
            SetPlayer1Pokemon(mVenusaurRegular.GetUri());
            SetPlayer2Pokemon(mVenusaurAgainst.GetUri());
        }

        private void PP1_MediaEnded(object sender, RoutedEventArgs e)
        {
            PokemonPlayer1.Position = new TimeSpan(0, 0, 1);
            PokemonPlayer1.Play();
        }

        private void PP1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("Could not load in .gif file!");
        }

        private void PP2_MediaEndend(object sender, RoutedEventArgs e)
        {
            PokemonPlayer2.Position = new TimeSpan(0, 0, 1);
            PokemonPlayer2.Play();
        }

        private void PP2_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("Could not load in .gif file!");
        }


        /// <summary>
        /// Self-made methods
        /// </summary>
        private void SetPlayer1Pokemon(Uri pokemonUri)
        {
            PokemonPlayer1.Source = pokemonUri;
            PokemonPlayer1.Play();
        }

        private void SetPlayer2Pokemon(Uri pokemonUri)
        {
            PokemonPlayer2.Source = pokemonUri;
            PokemonPlayer2.Play();
        }

    }
}
