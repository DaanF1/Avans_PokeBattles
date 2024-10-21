using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Avans_PokeBattles.Server;

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
        public string dirPrefix = System.AppDomain.CurrentDomain.BaseDirectory;
        public UriKind standardUriKind = UriKind.Absolute;

        // Media
        public MediaState PPlayer1State;
        public MediaState PPlayer2State;
        public MediaPlayer playerBattleMusic = new MediaPlayer();
        public MediaPlayer hitPlayer = new MediaPlayer();

        public LobbyWindow()
        {
            InitializeComponent();

            // Set name
            lblPlayer1Name.Content = ""; //playerName;

            // Play Music
            PlayMusic(playerBattleMusic, dirPrefix + "/Sounds/BattleMusic.wav", 30, true);
        }

        /// <summary>
        /// Generated methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: Load in Players Pokemon


            //// Set Preview Pokemon Player 1
            //P1Pokemon1Preview.Source = new BitmapImage(player1.GetPokemon(0).PreviewUri);
            //P1Pokemon2Preview.Source = new BitmapImage(player1.GetPokemon(1).PreviewUri);
            //P1Pokemon3Preview.Source = new BitmapImage(player1.GetPokemon(2).PreviewUri);
            //P1Pokemon4Preview.Source = new BitmapImage(player1.GetPokemon(3).PreviewUri);
            //P1Pokemon5Preview.Source = new BitmapImage(player1.GetPokemon(4).PreviewUri);
            //P1Pokemon6Preview.Source = new BitmapImage(player1.GetPokemon(5).PreviewUri);
            //// Set Preview Pokemon Player 2
            //P2Pokemon1Preview.Source = new BitmapImage(player2.GetPokemon(0).PreviewUri);
            //P2Pokemon2Preview.Source = new BitmapImage(player2.GetPokemon(1).PreviewUri);
            //P2Pokemon3Preview.Source = new BitmapImage(player2.GetPokemon(2).PreviewUri);
            //P2Pokemon4Preview.Source = new BitmapImage(player2.GetPokemon(3).PreviewUri);
            //P2Pokemon5Preview.Source = new BitmapImage(player2.GetPokemon(4).PreviewUri);
            //P2Pokemon6Preview.Source = new BitmapImage(player2.GetPokemon(5).PreviewUri);

            //// Set Battle Pokemon
            ////PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
            //SetPlayer1Pokemon(player1.GetPokemon(0).BattleForUri);
            //SetPlayer1PokemonHealth(player1.GetPokemon(0).Health);
            //LoadPokemonAttacks(player1.GetPokemon(0));
            ////PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
            //SetPlayer2Pokemon(player2.GetPokemon(0).BattleAgainstUri);
            //SetPlayer2PokemonHealth(player2.GetPokemon(0).Health);
            //// Start the game
            //Task.Run(() =>
            //{
            //    //StartGame();
            //});
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

        private void btnOption1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }

        private void btnOption2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }

        private void btnOption3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }

        private void btnOption4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }

        /// <summary>
        /// Self-made methods
        /// </summary>
        private async void GetServerMessages()
        {
           await Task.Run(() =>
           {

           });
        }
        
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
            }
        }

        private void RefreshMedia2Element()
        {
            // Update event for MediaElement of Player 2
            while (PPlayer2State == MediaState.Manual || PPlayer2State == MediaState.Play)
            {
                PPlayer2State = GetMediaState(PokemonPlayer2);
                // TODO: Prevent gif from leaving dots around
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

        //private async void StartGame()
        //{
        //    // Go through rounds
        //    for (int i = 1; i < int.MaxValue; i++)
        //    {
        //        lblRound.Content = "Round: " + i;

        //        // Set Battle Pokemon
        //        //PokemonPlayer1.RenderSize = new System.Windows.Size(50, 50);
        //        SetPlayer1Pokemon(player1.GetPokemon(i-1).BattleForUri);
        //        SetPlayer1PokemonHealth(player1.GetPokemon(i-1).Health);
        //        LoadPokemonAttacks(player1.GetPokemon(i-1));
        //        //PokemonPlayer2.RenderSize = new System.Windows.Size(50, 50);
        //        SetPlayer2Pokemon(player2.GetPokemon(0).BattleAgainstUri);
        //        SetPlayer2PokemonHealth(player2.GetPokemon(0).Health);

        //        SelectMove();
                
        //    }
        //}

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

        private System.Windows.Media.Brush GetTypeColor(Server.Type type)
        {
            // Return button color based on attack type
            switch (type)
            {
                case Server.Type.Normal:
                    return System.Windows.Media.Brushes.LightGray;
                case Server.Type.Fire:
                    return System.Windows.Media.Brushes.Red;
                case Server.Type.Water:
                    return System.Windows.Media.Brushes.LightBlue;
                case Server.Type.Grass:
                    return System.Windows.Media.Brushes.LightGreen;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Play a music file from the project
        /// </summary>
        /// <param name="path"></param>
        /// <param name="volume"></param>
        public void PlayMusic(MediaPlayer player, string path, double volume, bool loop)
        {
            if (path != null)
            {
                player.Open(new Uri(path));
                if (loop)
                {
                    player.MediaEnded += new EventHandler(Media_Ended);
                }
                // MediaPlayer volume is a float value between 0 and 1.
                player.Volume = volume / 100.0f;
                player.Play();
                return;
            }
        }

        private void Media_Ended(object sender, EventArgs e)
        {
            // Set time to zero (replay/ loop)
            playerBattleMusic.Position = TimeSpan.Zero;
            playerBattleMusic.Play();
        }

    }
}