using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;

namespace Avans_PokeBattles.Client
{
    /// <summary>
    /// Interaction logic for LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        // Important stuff:
        private readonly TcpClient tcpClient;
        private string namePlayer1;
        private string namePlayer2;

        // Uri prefixes for loading images
        public string dirPrefix = System.AppDomain.CurrentDomain.BaseDirectory;
        public UriKind standardUriKind = UriKind.Absolute;

        // Media
        public MediaState PPlayer1State;
        public MediaState PPlayer2State;
        public MediaPlayer playerBattleMusic = new();
        public MediaPlayer buttonPlayer = new();
        public MediaPlayer hitPlayer = new();

        // Other variables:
        private int pokemonIndex = 0;
        private int nameIndex = 1;

        public LobbyWindow(TcpClient client)
        {
            InitializeComponent();

            // Set name (should be overwritten later)
            lblPlayer1Name.Content = "Your team:";
            lblPlayer2Name.Content = "Oponent team:";
            tcpClient = client;

            // Play Music
            PlayMusic(playerBattleMusic, dirPrefix + "/Sounds/BattleMusic.wav", 30, true);
            GetServerMessages();
        }

        private async void GetServerMessages()
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[10000];
            while (tcpClient.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"CLIENT: Received from server: {message}");

                // Check if the message is a Player name
                if (message.StartsWith("Player") && !message.StartsWith("PlayerTeam") && !message.StartsWith("Player 1 used") && !message.StartsWith("Player 2 used"))
                {
                    int playerNumber = nameIndex;
                    if (playerNumber == 1)
                    {
                        // Name is from Player 1
                        string name = message.Split(':')[1].Trim('\n');
                        namePlayer1 = string.Concat(name.Substring(0, 1).ToUpper(), name.AsSpan(1));
                        lblPlayer1Name.Content = $"{name}'s team:";
                    } 
                    else
                    {
                        // Name is from Player 2
                        string name = message.Split(':')[1].Trim('\n');
                        namePlayer2 = string.Concat(name.Substring(0, 1).ToUpper(), name.AsSpan(1));
                        lblPlayer2Name.Content = $"{name}'s team:";
                    }
                    nameIndex++;
                }
                // Check if the message is a team info message
                else if (message.StartsWith("PlayerTeam"))
                {
                    List<Pokemon> pokemon = [];
                    // Get Pokemon of both teams
                    for (int i = 0; i < 6; i++)
                    {
                        Pokemon p = await GetServerPokemon(stream);
                        pokemon.Add(p);
                    }
                    DisplayTeams(pokemon);
                }
                else if (message.StartsWith("chat:"))
                {
                    // Display chat message
                    if (!string.IsNullOrWhiteSpace(txtReadChat.Text))
                    {
                        txtReadChat.Text += $"\n{namePlayer2}: {message.Split(':')[1]}";
                    }
                    else
                    {
                        txtReadChat.Text += $"{namePlayer2}: {message.Split(':')[1]}";
                    }
                }
                else
                {
                    // Handle other types of messages if necessary
                    Console.WriteLine($"CLIENT: Unhandled message: {message}");
                }
            }
        }

        /// <summary>
        /// Helper method to read serialized Pokemon objects.
        /// Made with help from ChatGPT!
        /// </summary>
        /// <param name="stream"></param>
        private static async Task<Pokemon> GetServerPokemon(NetworkStream stream)
        {
            while (true)
            {
                // Read the length of the incoming message
                byte[] lengthBytes = new byte[4];
                int bytesRead = await stream.ReadAsync(lengthBytes);
                if (bytesRead == 0) break; // End of stream

                int messageLength = BitConverter.ToInt32(lengthBytes, 0);

                // Read the actual message
                byte[] jsonBytes = new byte[messageLength];
                bytesRead = await stream.ReadAsync(jsonBytes);
                if (bytesRead == 0) break; // End of stream

                // Convert the JSON bytes to a string
                string jsonString = Encoding.UTF8.GetString(jsonBytes);

                // Deserialize the JSON string to a Pokemon object
                Pokemon receivedPokemon = JsonSerializer.Deserialize<Pokemon>(jsonString);

                // Display the deserialized object
                Console.WriteLine($"CLIENT: Received Pokemon: Name={receivedPokemon.Name}, Health={receivedPokemon.CurrentHealth}");

                return receivedPokemon;
            }
            return null; // If we even get here
        }

        private void DisplayTeams(List<Pokemon> pokemon)
        {
            foreach (Pokemon poke in pokemon)
            {
                // Set the preview images for Player 1 & Player 2
                Uri previewUri = new($"{dirPrefix}/Sprites/a{poke.Name}Preview.png", standardUriKind);
                switch (pokemonIndex)
                {
                    case 0: P1Pokemon1Preview.Source = new BitmapImage(previewUri); break;
                    case 1: P1Pokemon2Preview.Source = new BitmapImage(previewUri); break;
                    case 2: P1Pokemon3Preview.Source = new BitmapImage(previewUri); break;
                    case 3: P1Pokemon4Preview.Source = new BitmapImage(previewUri); break;
                    case 4: P1Pokemon5Preview.Source = new BitmapImage(previewUri); break;
                    case 5: P1Pokemon6Preview.Source = new BitmapImage(previewUri); break;
                    case 6: P2Pokemon1Preview.Source = new BitmapImage(previewUri); break;
                    case 7: P2Pokemon2Preview.Source = new BitmapImage(previewUri); break;
                    case 8: P2Pokemon3Preview.Source = new BitmapImage(previewUri); break;
                    case 9: P2Pokemon4Preview.Source = new BitmapImage(previewUri); break;
                    case 10: P2Pokemon5Preview.Source = new BitmapImage(previewUri); break;
                    case 11: P2Pokemon6Preview.Source = new BitmapImage(previewUri); break;
                }

                // Load the For.gif for the first Pokémon of Player 1 into PokemonPlayer1 MediaElement
                if (pokemonIndex == 0) // Ensures it only sets the first Pokémon
                {
                    Uri forGifUri = new($"{dirPrefix}/Sprites/a{poke.Name}For.gif", standardUriKind);
                    SetPlayer1Pokemon(forGifUri);
                    SetPlayer1PokemonHealth(poke.CurrentHealth);
                    LoadPokemonAttacks(poke);
                }

                // Load the Against.gif for the first Pokémon of Player 2 into PokemonPlayer2 MediaElement
                if (pokemonIndex == 6) // Ensures it only sets the first Pokémon
                {
                    Uri againstGifUri = new Uri($"{dirPrefix}/Sprites/a{poke.Name}Against.gif", standardUriKind);
                    SetPlayer2Pokemon(againstGifUri);
                    SetPlayer2PokemonHealth(poke.CurrentHealth);
                }
                pokemonIndex++;
            }
        }

        /// <summary>
        /// Generated methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle turn starting:

        }

        // Setting items:
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

        private static Brush GetTypeColor(Server.Type type)
        {
            // Return button color based on attack type
            switch (type)
            {
                case Server.Type.Normal:
                    return Brushes.LightGray;
                case Server.Type.Fire:
                    return Brushes.Red;
                case Server.Type.Water:
                    return Brushes.LightBlue;
                case Server.Type.Grass:
                    return Brushes.LightGreen;
                default:
                    return null;
            }
        }

        // Media:
        private void PP1_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Replay gif animation
            PokemonPlayer1.RenderSize = new Size(50, 50);
            PPlayer1State = MediaState.Stop;
            PokemonPlayer1.Position = new TimeSpan(0, 0, 1);
            PokemonPlayer1.Play();
        }
        private void PP1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("CLIENT: Could not load in .gif file!");
        }

        private void PP2_MediaEndend(object sender, RoutedEventArgs e)
        {
            // Replay gif animation
            PokemonPlayer2.RenderSize = new Size(50, 50);
            PPlayer2State = MediaState.Stop;
            PokemonPlayer2.Position = new TimeSpan(0, 0, 1);
            PokemonPlayer2.Play();
        }
        private void PP2_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("CLIENT: Could not load in .gif file!");
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

        // Preview sound of attacks:
        private void btnOption1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(buttonPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }
        private void btnOption2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(buttonPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }
        private void btnOption3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(buttonPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }
        private void btnOption4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayMusic(buttonPlayer, dirPrefix + "/Sounds/AttackButton.wav", 50, false);
        }

        // Hit events:
        private async void SendMoveToServer(string moveName)
        {
            try
            {
                // Create move message format
                string moveMessage = $"move:{moveName}";

                // Convert message to bytes and send to server
                byte[] moveBytes = Encoding.UTF8.GetBytes(moveMessage);
                await tcpClient.GetStream().WriteAsync(moveBytes);

                Console.WriteLine($"CLIENT: Sent move to server: {moveMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CLIENT: Failed to send move: {ex.Message}");
            }
        }

        // Attack buttons:
        private void btnOption1_Click(object sender, RoutedEventArgs e)
        {
            // Send move selected from btnOption1
            SendMoveToServer(btnOption1.Content.ToString());
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/Hit.wav", 50, false);
        }
        private void btnOption2_Click(object sender, RoutedEventArgs e)
        {
            SendMoveToServer(btnOption2.Content.ToString());
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/Hit.wav", 50, false);
        }
        private void btnOption3_Click(object sender, RoutedEventArgs e)
        {
            SendMoveToServer(btnOption3.Content.ToString());
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/Hit.wav", 50, false);
        }
        private void btnOption4_Click(object sender, RoutedEventArgs e)
        {
            SendMoveToServer(btnOption4.Content.ToString());
            PlayMusic(hitPlayer, dirPrefix + "/Sounds/Hit.wav", 50, false);
        }

        // Chatting:
        private void btnSendChat_Clicked(object sender, RoutedEventArgs e)
        {
            // Handle chatting / displaying rounds
            if (txtTypeChat.Text.Length > 0 && !txtTypeChat.Text.Equals("Type something..."))
            {
                if (!string.IsNullOrWhiteSpace(txtReadChat.Text))
                {
                    txtReadChat.Text += $"\n{namePlayer1}: {txtTypeChat.Text}";
                }
                else
                {
                    txtReadChat.Text += $"{namePlayer1}: {txtTypeChat.Text}";
                }
                // Send chat to server
                SendChatToServer(txtTypeChat.Text);
                txtTypeChat.Text = "Type something...";
            }
        }
        private void txtTypeChat_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtTypeChat.Text = "";
        }
        private async void SendChatToServer(string chatMessage)
        {
            try
            {
                // Create chat message format
                string chat = $"chat:{chatMessage}";

                // Convert chat message to bytes and send to server
                byte[] chatBytes = Encoding.UTF8.GetBytes(chat);
                await tcpClient.GetStream().WriteAsync(chatBytes);

                Console.WriteLine($"CLIENT: Sent chat to server: {chat}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CLIENT: Failed to send chat: {ex.Message}");
            }
        }

        // FileIO chatlogs:
        private void btnCreateChatlog_Clicked(object sender, RoutedEventArgs e)
        {
            // Create file (at Avans_PokeBattles\Avans_PokeBattles\bin\Debug\net8.0-windows\Chatlogs directory)
            string currentTime = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            string path = AppDomain.CurrentDomain.BaseDirectory + "Chatlogs\\Chatlog-" + currentTime + ".txt";
            if (!File.Exists(path))
            {
                var logFile = File.Create(path);
                logFile.Close();
            }
            // Write to file
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine("---Chatlog---");
                foreach (string line in txtReadChat.Text.Split("\n"))
                {
                    outputFile.WriteLine(line);
                } 
                outputFile.WriteLine("---End of Chatlog---");
            }
            // Confirm in chat
            if (!string.IsNullOrWhiteSpace(txtReadChat.Text))
            {
                txtReadChat.Text += $"\nServer: Chatlog created!";
            }
            else
            {
                txtReadChat.Text += $"Server: Chatlog created!";
            }

        }
        private void btnReadChatlog_Clicked(object sender, RoutedEventArgs e)
        {
            // Check file DateTime (at: (Project_Name)\Avans_PokeBattles\Avans_PokeBattles\bin\Debug\net8.0-windows\Chatlogs directory)
            DateTime currentDateTime = DateTime.Now;
            string currentString = currentDateTime.ToString("dd-MM-yyyy_HH-mm-ss"); // Format DateTime

            string pathToLogs = AppDomain.CurrentDomain.BaseDirectory + "Chatlogs";
            string[] pathToFiles = Directory.GetFiles(pathToLogs);
            DateTime[] dateTimes = new DateTime[pathToFiles.Length];
            for (int i = 0; i < pathToFiles.Length; i++)
            {
                string fileName = pathToFiles[i].Split("\\").Last();
                DateTime fileDateTime = DateTime.ParseExact(fileName.Substring(8, 19), "dd-MM-yyyy_HH-mm-ss", CultureInfo.InvariantCulture); // Format DateTime
                dateTimes[i] = fileDateTime;
            }

            DateTime latestDate = DateTime.MinValue;
            if (dateTimes != null)
            {
                // Filter on today's most recent chatlog
                latestDate = dateTimes.Where(r => r.Year.Equals(currentDateTime.Year) &&
                    r.Month.Equals(currentDateTime.Month) &&
                    r.Day.Equals(currentDateTime.Day)).Max();
            }
            
            // Print the most recent chatlog in the chat
            if (latestDate != DateTime.MinValue)
            {
                string latestString = latestDate.ToString("dd-MM-yyyy_HH-mm-ss"); // Format DateTime
                txtReadChat.Text += $"\nServer: Loading chatlog from {latestString}!";

                string recentFilePath = AppDomain.CurrentDomain.BaseDirectory + "Chatlogs\\Chatlog-" + latestString + ".txt";
                using (StreamReader sr = new StreamReader(recentFilePath))
                {
                    string line = "not null";
                    while (line != null)
                    {
                        line = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(txtReadChat.Text))
                        {
                            txtReadChat.Text += $"\n{line}";
                        }
                        else
                        {
                            txtReadChat.Text += line;
                        }
                    }
                }
            }
            else {
                txtReadChat.Text += $"\nServer: No recent chatlog found!";
            }
        }

        // Helper methods for refreshing MediaElements:
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

    }
}