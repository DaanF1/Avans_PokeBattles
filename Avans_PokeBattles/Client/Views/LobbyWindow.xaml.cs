using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        // Players
        private Player player1;
        private Player player2;

        public LobbyWindow()
        {
            InitializeComponent();

            // Set name
            lblPlayer1Name.Content = ""; //playerName;

            // Create Pokemon & Moves
            List<Move> unownMoves = new List<Move>();
            unownMoves.Add(new Move("Hidden Power", 60, 100, Type.Normal));
            unownMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            unownMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            unownMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            Pokemon unown = new Pokemon("Unown", new Uri(uriPrefix + "/Sprites/aUnownPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aUnownFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aUnownAgainst.gif", standardUriKind), unownMoves, 80, 90);

            List<Move> venusaurMoves = new List<Move>();
            venusaurMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            venusaurMoves.Add(new Move("Take Down", 90, 85, Type.Normal));
            venusaurMoves.Add(new Move("Razor Leaf", 55, 95, Type.Grass));
            venusaurMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(uriPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), venusaurMoves, 195, 70);

            List<Move> charizardMoves = new List<Move>();
            charizardMoves.Add(new Move("Scratch", 40, 100, Type.Normal));
            charizardMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            charizardMoves.Add(new Move("Slash", 70, 100, Type.Normal));
            charizardMoves.Add(new Move("Flamethrower", 90, 100, Type.Fire));
            Pokemon charizard = new Pokemon("Charizard", new Uri(uriPrefix + "/Sprites/aCharizardPreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aCharizardFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aCharizardAgainst.gif", standardUriKind), charizardMoves, 125, 120);

            List<Move> blastoiseMoves = new List<Move>();
            blastoiseMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            blastoiseMoves.Add(new Move("Aqua Tail", 90, 90, Type.Water));
            blastoiseMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            blastoiseMoves.Add(new Move("Rapid Spin", 50, 100, Type.Normal));
            Pokemon blastoise = new Pokemon("Blastoise", new Uri(uriPrefix + "/Sprites/aBlastoisePreview.png", standardUriKind), new Uri(uriPrefix + "/Sprites/aBlastoiseFor.gif", standardUriKind), new Uri(uriPrefix + "/Sprites/aBlastoiseAgainst.gif", standardUriKind), blastoiseMoves, 145, 75);

            // Add Pokemon to the lister
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
            List<Pokemon> player1Pokemon = new List<Pokemon>();
            for (int i = 0; i < 6; i++)
            {
                player1Pokemon.Add(lister.GetRandomPokemon());
            }
            player1 = new Player("", player1Pokemon);

            // Set Preview Pokemon Player 1
            for (int i = 0; i < 6; i++)
            {
                Image previewImage = new Image();
                previewImage.Source = new BitmapImage(player1.GetPokemon(i).PreviewUri);
            }
            //foreach (var child in cvGamePanel.Children)
            //{
            //    if (child.GetType() == Image)
            //    {

            //    }
            //}
            P1Pokemon1Preview.Source = new BitmapImage(player1.GetPokemon(0).PreviewUri);
            P1Pokemon2Preview.Source = new BitmapImage(player1.GetPokemon(1).PreviewUri);
            P1Pokemon3Preview.Source = new BitmapImage(player1.GetPokemon(2).PreviewUri);
            P1Pokemon4Preview.Source = new BitmapImage(player1.GetPokemon(3).PreviewUri);
            P1Pokemon5Preview.Source = new BitmapImage(player1.GetPokemon(4).PreviewUri);
            P1Pokemon6Preview.Source = new BitmapImage(player1.GetPokemon(5).PreviewUri);

            // Set Random Pokemon Player 2
            List<Pokemon> player2Pokemon = new List<Pokemon>();
            for (int i = 0; i < 6; i++)
            {
                player2Pokemon.Add(lister.GetRandomPokemon());
            }
            player2 = new Player("", player2Pokemon);

            // Set Preview Pokemon Player 2
            P2Pokemon1Preview.Source = new BitmapImage(player2.GetPokemon(0).PreviewUri);
            P2Pokemon2Preview.Source = new BitmapImage(player2.GetPokemon(1).PreviewUri);
            P2Pokemon3Preview.Source = new BitmapImage(player2.GetPokemon(2).PreviewUri);
            P2Pokemon4Preview.Source = new BitmapImage(player2.GetPokemon(3).PreviewUri);
            P2Pokemon5Preview.Source = new BitmapImage(player2.GetPokemon(4).PreviewUri);
            P2Pokemon6Preview.Source = new BitmapImage(player2.GetPokemon(5).PreviewUri);

            // Set Battle Pokemon
            //PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer1Pokemon(player1.GetPokemon(0).BattleForUri);
            SetPlayer1PokemonHealth(player1.GetPokemon(0).Health);
            LoadPokemonAttacks(player1.GetPokemon(0));
            //PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
            SetPlayer2Pokemon(player2.GetPokemon(0).BattleAgainstUri);
            SetPlayer2PokemonHealth(player2.GetPokemon(0).Health);
            // Start the game
            Task.Run(() =>
            {
                //StartGame();
            });
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

        private void SetPlayer1PokemonHealth(int health)
        {
            lblP1PokemonHealth.Content = "Health: " + health.ToString();
        }

        private void SetPlayer2Pokemon(Uri pokemonUri)
        {
            // Set MediaElement to gif
            PokemonPlayer2.Source = pokemonUri;
            PokemonPlayer2.Play();
            Task.Run(() => { RefreshMedia2Element(); }); // Start refreshing the MediaElement
        }

        private void SetPlayer2PokemonHealth(int health)
        {
            lblP2PokemonHealth.Content = "Health: " + health.ToString();
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

        private async void StartGame()
        {
            // Go through rounds
            for (int i = 1; i < int.MaxValue; i++)
            {
                lblRound.Content = "Round: " + i;

                // Set Battle Pokemon
                //PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
                SetPlayer1Pokemon(player1.GetPokemon(i-1).BattleForUri);
                SetPlayer1PokemonHealth(player1.GetPokemon(i-1).Health);
                LoadPokemonAttacks(player1.GetPokemon(i-1));
                //PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
                SetPlayer2Pokemon(player2.GetPokemon(0).BattleAgainstUri);
                SetPlayer2PokemonHealth(player2.GetPokemon(0).Health);

                SelectMove();
                
            }
        }

        private void LoadPokemonAttacks(Pokemon pokemon)
        {
            btnOption1.Content = pokemon.GetMove(0).MoveName;
            btnOption1.Background = GetTypeColor(pokemon.GetMove(0).TypeOfAttack);
            btnOption2.Content = pokemon.GetMove(1).MoveName;
            btnOption2.Background = GetTypeColor(pokemon.GetMove(1).TypeOfAttack);
            btnOption3.Content = pokemon.GetMove(2).MoveName;
            btnOption3.Background = GetTypeColor(pokemon.GetMove(2).TypeOfAttack);
            btnOption4.Content = pokemon.GetMove(3).MoveName;
            btnOption4.Background = GetTypeColor(pokemon.GetMove(3).TypeOfAttack);
        }

        private System.Windows.Media.Brush GetTypeColor(Type type)
        {
            switch (type)
            {
                case Type.Normal:
                    return System.Windows.Media.Brushes.LightGray;
                case Type.Fire:
                    return System.Windows.Media.Brushes.Red;
                case Type.Water:
                    return System.Windows.Media.Brushes.LightBlue;
                case Type.Grass:
                    return System.Windows.Media.Brushes.LightGreen;
                default:
                    return null;
            }
        }

        private async Task SelectMove()
        {
            while (true)
            {
                if (btnOption1.IsPressed)
                {

                }
                else if (btnOption2.IsPressed)
                {

                }
                else if (btnOption3.IsPressed)
                {

                }
                else if (btnOption4.IsPressed)
                {

                }
            }
        }

    }
}