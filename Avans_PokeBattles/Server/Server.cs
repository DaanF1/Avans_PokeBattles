using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Avans_PokeBattles.Server
{
    public class Server
    {
        private static TcpListener listener;
        private static readonly int port = 8000;
        private static LobbyManager lobbyManager = new();
        private static Lobby connectedLobby;
        private static readonly Dictionary<TcpClient, string> clientNames = [];
        private static readonly ProfileManager profileManager = new ProfileManager();
        private static readonly PokemonLister pokemonLister = new PokemonLister();
        private static bool isRunning = false;
        public static string dirPrefix = AppDomain.CurrentDomain.BaseDirectory; 
        public static UriKind standardUriKind = UriKind.Absolute; 

        public static async void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;

            InitializePokemonLister();

            Console.WriteLine("SERVER: Server started. Waiting for connections...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("SERVER: Client connected.");
                Task.Run(() => HandleClientAsync(client));
            }
        }

        public static void Stop()
        {
            isRunning = false;
            Stop(); // Stop the Server
        }

        public static bool IsRunning()
        {
            return isRunning;
        }

        // Handle individual client requests
        private static async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[10000];
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"SERVER: Received message: {message}");

                // Handle getting names
                if (message.StartsWith("Player name:"))
                {
                    string name = message.Split(':')[1].Trim();
                }
                // Handle join-lobby request
                if (message.StartsWith("join-lobby"))
                {
                    string[] splitMessage = message.Split(':');
                    if (splitMessage.Length == 2)
                    {
                        // Get current Player name
                        string namePlayer1 = "";
                        int index = 0;
                        foreach (TcpClient tcp in clientNames.Keys)
                        {
                            if (tcp == client)
                                namePlayer1 = clientNames.Values.ElementAt(index);
                            index++;
                        }

                        string lobbyId = splitMessage[1];
                        bool joined = lobbyManager.TryJoinLobby(lobbyId, client, namePlayer1);

                        // Notify the client if joining the lobby was successful
                        string responseMessage = joined ? "lobby-joined" : "lobby-full";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await stream.WriteAsync(response, 0, response.Length);

                        // If the lobby is now full, start the game
                        if (joined && lobbyManager.GetCurrentLobby(int.Parse(lobbyId.Split('-')[1])).IsFull)
                        {
                            connectedLobby = lobbyManager.GetCurrentLobby(int.Parse(lobbyId.Split('-')[1]));
                            Console.WriteLine("SERVER: Lobby is full, starting the game...");
                            connectedLobby.StartGame(); // Only start the game when lobby is confirmed full
                        }
                    }
                }
                else if (message.StartsWith("create-team"))
                {
                    //Format: create-team:pokemon1,pokemon2,pokemon3,...
                    string[] parts = message.Split(":");
                    string[] pokemonNames = parts[1].Split(",");
                    var profile = profileManager.GetOrCreateProfile(clientNames[client]);
                    profile.RemoveTeam();

                    foreach (var pokemonName in pokemonNames) 
                    {
                        Pokemon pokemon = pokemonLister.GetPokemon(pokemonName.Trim());
                        profile.AddPokemonToTeam(pokemon);
                    }
                }
                else if (message.StartsWith("move:"))
                {
                    // Handle move command by forwarding it to the appropriate lobby
                    var lobby = lobbyManager.GetLobbyForClient(client);
                    if (lobby != null)
                    {
                        Console.WriteLine($"SERVER: Forwarding move to Lobby {lobby.LobbyId}");
                        await lobby.HandleMove(message, client);
                    }
                    else
                    {
                        Console.WriteLine("SERVER: Client not assigned to any lobby.");
                    }
                }
                else if (message.StartsWith("chat:")) 
                {
                    var lobby = lobbyManager.GetLobbyForClient(client);
                    if (lobby != null)
                    {
                        Console.WriteLine($"SERVER: Forwarding message to Lobby {lobby.LobbyId}");
                        await lobby.HandleChat(message, client);
                    }
                    else
                    {
                        Console.WriteLine("SERVER: Client not assigned to any lobby.");
                    }
                }
            }
            client.Close();
        }

        // Get the lobby manager instance
        internal static LobbyManager GetLobbymanager()
        {
            return lobbyManager;
        }

        private static void InitializePokemonLister()
        {
            // Unown
            List<Move> unownMoves =
            [
                new Move("Solar Beam", 120, 100, Type.Grass, StatusEffect.None, 0, 0),
                new Move("Inferno", 100, 50, Type.Fire, StatusEffect.Burn, 30, 0), // 30% chance to burn
                new Move("Hidden Power", 90, 90, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Hydro Pump", 110, 80, Type.Water, StatusEffect.None, 0, 0)
            ];
            Pokemon unown = new("Unown",
                new Uri(dirPrefix + "Sprites/aUnownPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aUnownFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aUnownAgainst.gif", standardUriKind),
                Type.Normal, 80, 110, unownMoves);
            // Venusaur
            List<Move> venusaurMoves =
            [
                new Move("Solar Beam", 120, 100, Type.Grass, StatusEffect.None, 0, 0),
                new Move("Take Down", 90, 85, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Razor Leaf", 55, 95, Type.Grass, StatusEffect.Poison, 20, 0), // 20% chance to poison
                new Move("Tackle", 40, 100, Type.Normal, StatusEffect.None, 0, 0)
            ];
            Pokemon venusaur = new("Venusaur",
                new Uri(dirPrefix + "Sprites/aVenusaurPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aVenusaurFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aVenusaurAgainst.gif", standardUriKind),
                Type.Grass, 195, 70, venusaurMoves);
            // Charizard
            List<Move> charizardMoves =
            [
                new Move("Scratch", 40, 100, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Inferno", 100, 50, Type.Fire, StatusEffect.Burn, 30, 0), // 30% chance to burn
                new Move("Slash", 70, 100, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Flamethrower", 90, 100, Type.Fire, StatusEffect.Burn, 10, 0) // 10% chance to burn
            ];
            Pokemon charizard = new("Charizard",
                new Uri(dirPrefix + "Sprites/aCharizardPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aCharizardFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aCharizardAgainst.gif", standardUriKind),
                Type.Fire, 125, 120, charizardMoves);
            // Blastoise
            List<Move> blastoiseMoves =
            [
                new Move("Hydro Pump", 110, 80, Type.Water, StatusEffect.None, 0, 0),
                new Move("Aqua Tail", 90, 90, Type.Water, StatusEffect.None, 0, 0),
                new Move("Tackle", 40, 100, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Rapid Spin", 50, 100, Type.Normal, StatusEffect.None, 0, 0)
            ];
            Pokemon blastoise = new("Blastoise",
                new Uri(dirPrefix + "Sprites/aBlastoisePreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aBlastoiseFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aBlastoiseAgainst.gif", standardUriKind),
                Type.Water, 145, 75, blastoiseMoves);
            // Pikachu
            List<Move> pikachuMoves =
            [
                new Move("Thunderbolt", 90, 100, Type.Normal, StatusEffect.Paralysis, 30, 0), // 30% chance to paralyze
                new Move("Quick Attack", 40, 100, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Iron Tail", 100, 75, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Electro Ball", 80, 100, Type.Normal, StatusEffect.None, 0, 0)
            ];
            Pokemon pikachu = new("Pikachu",
                new Uri(dirPrefix + "Sprites/aPikachuPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aPikachuFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aPikachuAgainst.gif", standardUriKind),
                Type.Normal, 100, 120, pikachuMoves);
            // Snorlax
            List<Move> snorlaxMoves =
            [
                new Move("Body Slam", 85, 100, Type.Normal, StatusEffect.Paralysis, 30, 0), // 30% chance to paralyze
                new Move("Rest", 0, 100, Type.Normal, StatusEffect.None, 100, 100), // No status effect, but restores HP
                new Move("Hyper Beam", 150, 90, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Headbutt", 70, 100, Type.Normal, StatusEffect.None, 0, 0)
            ];
            Pokemon snorlax = new("Snorlax",
                new Uri(dirPrefix + "Sprites/aSnorlaxPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aSnorlaxFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aSnorlaxAgainst.gif", standardUriKind),
                Type.Normal, 200, 30, snorlaxMoves);
            // Gengar
            List<Move> gengarMoves =
            [
                new Move("Shadow Ball", 80, 100, Type.Normal, StatusEffect.None, 0, 0),
                new Move("Dream Eater", 100, 100, Type.Normal, StatusEffect.Sleep, 100, 0), // 100% chance to put target to sleep
                new Move("Sludge Bomb", 90, 100, Type.Normal, StatusEffect.Poison, 30, 0), // 30% chance to poison
                new Move("Nightmare", 0, 100, Type.Normal, StatusEffect.None, 0, 0)
            ];
            Pokemon gengar = new("Gengar",
                new Uri(dirPrefix + "Sprites/aGengarPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aGengarFor.gif", standardUriKind),
                new Uri(dirPrefix + "Sprites/aGengarAgainst.gif", standardUriKind),
                Type.Normal, 150, 110, gengarMoves);

            // Add all Pokemon to Lister
            pokemonLister.AddAllPokemon([unown, venusaur, charizard, blastoise, pikachu, snorlax, gengar]);
        }

        public static PokemonLister GetPokemonLister()
        {
            return pokemonLister;
        }
    }
}