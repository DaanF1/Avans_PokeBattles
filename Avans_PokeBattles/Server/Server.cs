using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Avans_PokeBattles.Server
{
    public class Server
    {
        private static TcpListener listener;
        private static int port = 8000;
        private TcpClient player1;
        private TcpClient player2;
        private string player1Name = null;
        private string player2Name = null;
        private PokemonLister pokemonLister = new PokemonLister();

        public async void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            AddInitialPokemon(pokemonLister);
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                if (player1 == null)
                {
                    player1 = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Player 1 connected.");
                    Task.Run(() => HandleClient(player1, 1));
                }
                else if (player2 == null)
                {
                    player2 = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Player 2 connected.");
                    Task.Run(() => HandleClient(player2, 2));
                }
            }
        }

        private async Task HandleClient(TcpClient client, int playerNumber)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1500];

            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Message from player {playerNumber}: {message}");

                if (message.StartsWith("Player name: "))
                {
                    string playerName = message.Substring("Player name: ".Length);

                    if (playerNumber == 1)
                        player1Name = playerName;
                    else
                        player2Name = playerName;

                    Console.WriteLine($"Player {playerNumber} name set to {playerName}");

                    // If both players have entered their names, notify them to start the game
                    if (player1Name != null && player2Name != null)
                    {
                        Task.Run(() => StartGame());
                    }
                }
            }

            // Handle disconnection
            client.Close();
        }

        private async Task StartGame()
        {
            // Randomly assign teams to both players
            var player1Team = RandomizeTeam(pokemonLister);
            var player2Team = RandomizeTeam(pokemonLister);

            // Serialize the teams
            string player1TeamSerialized = SerializeTeam(player1Team);
            string player2TeamSerialized = SerializeTeam(player2Team);

            NetworkStream stream1 = player1.GetStream();
            NetworkStream stream2 = player2.GetStream();

            // Notify both players to open their game window and send the teams
            await SendMessage(stream1, $"start-game:Player 1:{player1Name},Player 2:{player2Name}");
            await SendMessage(stream2, $"start-game:Player 1:{player1Name},Player 2:{player2Name}");

            // Send team data to both players
            Console.WriteLine($"Sending to Player 1: Player 1 team:{player1TeamSerialized}");
            await SendMessage(stream1, $"Player 1 team:{player1TeamSerialized}");
            Console.WriteLine($"Sending to Player 2: Player 2 team:{player2TeamSerialized}");
            await SendMessage(stream2, $"Player 2 team:{player2TeamSerialized}");

            Task.Run(() => HandleGameLogic(player1, stream1, player2, stream2));
        }

        private void AddInitialPokemon(PokemonLister lister)
        {
            string baseUri = System.AppDomain.CurrentDomain.BaseDirectory + "Images/";

            var venusaur = new Pokemon(
        "Venusaur",
        new Uri(baseUri + "mVenusaurPreview.png", UriKind.Absolute),
        new Uri(baseUri + "mVenusaurFor.gif", UriKind.Absolute),
        new Uri(baseUri + "mVenusaurAgainst.gif", UriKind.Absolute),
        new List<Move>
        {
            new Move("Tackle", 40, 100, Type.Normal),
            new Move("Vine Whip", 45, 100, Type.Grass),
            new Move("Razor Leaf", 55, 95, Type.Grass),
            new Move("Solar Beam", 120, 100, Type.Grass)
        },
        200,
        80
    );

            var charizard = new Pokemon(
                "Charizard",
                new Uri(baseUri + "aCharizardPreview.png", UriKind.Absolute),
                new Uri(baseUri + "aCharizardFor.gif", UriKind.Absolute),
                new Uri(baseUri + "aCharizardAgainst.gif", UriKind.Absolute),
                new List<Move>
                {
            new Move("Scratch", 40, 100, Type.Normal),
            new Move("Flamethrower", 90, 100, Type.Fire),
            new Move("Fire Spin", 35, 85, Type.Fire),
            new Move("Dragon Claw", 80, 100, Type.Fire)
                },
                200,
                100
            );

            var blastoise = new Pokemon(
                "Blastoise",
                new Uri(baseUri + "aBlastoisePreview.png", UriKind.Absolute),
                new Uri(baseUri + "aBlastoiseFor.gif", UriKind.Absolute),
                new Uri(baseUri + "aBlastoiseAgainst.gif", UriKind.Absolute),
                new List<Move>
                {
            new Move("Tackle", 40, 100, Type.Normal),
            new Move("Water Gun", 40, 100, Type.Water),
            new Move("Hydro Pump", 110, 80, Type.Water),
            new Move("Surf", 90, 100, Type.Water)
                },
                200,
                70
            );

            // Add Pokémon to the list
            lister.AddPokemon(venusaur);
            lister.AddPokemon(charizard);
            lister.AddPokemon(blastoise);
        }

        private List<Pokemon> RandomizeTeam(PokemonLister lister)
        {
            var team = new List<Pokemon>();

            team.Add(lister.GetPokemon("Venusaur"));
            team.Add(lister.GetPokemon("Charizard"));
            team.Add(lister.GetPokemon("Blastoise"));
            team.Add(lister.GetPokemon("Venusaur"));
            team.Add(lister.GetPokemon("Charizard"));
            team.Add(lister.GetPokemon("Blastoise"));

            // Shuffle the team
            return team.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private string SerializeTeam(List<Pokemon> team)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Pokemon>));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, team);
                return writer.ToString();
            }
        }

        private async Task HandleGameLogic(TcpClient player1, NetworkStream stream1, TcpClient player2, NetworkStream stream2)
        {
            byte[] buffer = new byte[1500];

            // Game logic handling between player 1 and player 2
            // Process moves, game state, etc.
            while (player1.Connected && player2.Connected)
            {
                // Example logic for move handling can be inserted here
            }

            player1.Close();
            player2.Close();
        }

        private async Task SendMessage(NetworkStream stream, string message)
        {
            byte[] response = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(response, 0, response.Length);
            await stream.FlushAsync();
        }
    }
}
