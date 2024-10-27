using System.Buffers.Text;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace Avans_PokeBattles.Server
{
    public class Lobby
    {
        private TcpClient player1;
        private TcpClient player2;
        private NetworkStream stream1;
        private NetworkStream stream2;

        private bool isPlayer1Turn = true;  // Track whose turn it is
        private PokemonLister pokemonLister = new PokemonLister();  // List of available Pokémon to pick from
        public string dirPrefix = System.AppDomain.CurrentDomain.BaseDirectory; // Directory prefix for files
        public UriKind standardUriKind = UriKind.Absolute; // Always get the absolute path

        public string LobbyId { get; private set; }
        public bool IsFull { get; private set; }

        public Lobby(string lobbyId)
        {
            LobbyId = lobbyId;
            IsFull = false;
            FillPokemonLister();  // Fill Pokémon lister with all available Pokemon
        }

        private void FillPokemonLister()
        {
            // Create a small list of available Pokémon (Venusaur, Charizard, Blastoise)
            List<Move> unownMoves = new List<Move>();
            unownMoves.Add(new Move("Hidden Power", 60, 100, Type.Normal));
            unownMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            unownMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            unownMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            Pokemon unown = new Pokemon("Unown", new Uri(dirPrefix + "/Sprites/aUnownPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aUnownFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aUnownAgainst.gif", standardUriKind), Type.Normal, 80, 90, unownMoves);

            List<Move> venusaurMoves = new List<Move>();
            venusaurMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            venusaurMoves.Add(new Move("Take Down", 90, 85, Type.Normal));
            venusaurMoves.Add(new Move("Razor Leaf", 55, 95, Type.Grass));
            venusaurMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), Type.Grass, 195, 70, venusaurMoves);

            List<Move> charizardMoves = new List<Move>();
            charizardMoves.Add(new Move("Scratch", 40, 100, Type.Normal));
            charizardMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            charizardMoves.Add(new Move("Slash", 70, 100, Type.Normal));
            charizardMoves.Add(new Move("Flamethrower", 90, 100, Type.Fire));
            Pokemon charizard = new Pokemon("Charizard", new Uri(dirPrefix + "/Sprites/aCharizardPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aCharizardFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aCharizardAgainst.gif", standardUriKind), Type.Fire, 125, 120, charizardMoves);

            List<Move> blastoiseMoves = new List<Move>();
            blastoiseMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            blastoiseMoves.Add(new Move("Aqua Tail", 90, 90, Type.Water));
            blastoiseMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            blastoiseMoves.Add(new Move("Rapid Spin", 50, 100, Type.Normal));
            Pokemon blastoise = new Pokemon("Blastoise", new Uri(dirPrefix + "/Sprites/aBlastoisePreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aBlastoiseFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aBlastoiseAgainst.gif", standardUriKind), Type.Water, 145, 75, blastoiseMoves);

            // Add Pokemon to the lister
            pokemonLister.AddPokemon(unown);
            pokemonLister.AddPokemon(venusaur);
            pokemonLister.AddPokemon(charizard);
            pokemonLister.AddPokemon(blastoise);
        }

        public void AddPlayer(TcpClient client)
        {
            if (player1 == null)
            {
                player1 = client;
                stream1 = client.GetStream();
            }
            else if (player2 == null)
            {
                player2 = client;
                stream2 = client.GetStream();
                IsFull = true;  // Lobby is full when both players have joined
            }
        }

        public async void StartGame()
        {
            // Assign random teams of 6 Pokémon to both players (allowing duplicates)
            List<Pokemon> player1Team = AssignRandomTeam();
            List<Pokemon> player2Team = AssignRandomTeam();

            Console.WriteLine("LOBBY: Sending 'start-game' signal to both players...");

            await SendMessage(stream1, "start-game");
            await SendMessage(stream2, "start-game");

            // Send teams to both players
            await SendTeam(stream1, player1Team, player2Team, 1);
            await SendTeam(stream2, player2Team, player1Team, 2);

            Console.WriteLine("LOBBY: Start-game messages sent to both players.");

        }

        private List<Pokemon> AssignRandomTeam()
        {
            // Select 6 Pokémon randomly
            List<Pokemon> teamOfPlayer = new List<Pokemon>();
            for (int i = 0; i < 6; i++)
            {
                Pokemon randomPokemon = pokemonLister.GetRandomPokemon(); // Allow duplicate Pokemon to be in the same team
                teamOfPlayer.Add(randomPokemon);
            }

            return teamOfPlayer;
        }

        public bool HasClient(TcpClient client)
        {
            return client == player1 || client == player2;
        }

        private async Task SendTeam(NetworkStream stream, List<Pokemon> playerTeam, List<Pokemon> opponentTeam, int playerNumber)
        {
            StringBuilder teamMessage = new StringBuilder();

            // Indicate sending a Player's team:
            teamMessage.Append($"PlayerTeam {playerNumber} team:\n");
            await SendMessage(stream, teamMessage.ToString());

            // Send the team now:
            foreach (Pokemon pokemon in playerTeam)
            {
                await SendPokemon(stream, pokemon);
            }

            // Indicate sending the opponent's team:
            teamMessage.Append($"Opponent team:\n");
            await SendMessage(stream, teamMessage.ToString());

            foreach (Pokemon pokemon in opponentTeam)
            {
                await SendPokemon(stream, pokemon);
            }
        }

        private bool IsMoveMessage(string message)
        {
            return message.StartsWith("move:") || message.StartsWith(" move:");  // Simplified check for move commands (possible to be expanded upon later)
        }

        public async Task HandleMove(string move, TcpClient sender)
        {
            Console.WriteLine("HANDLING MOVE");
            // Identify which player sent the move
            NetworkStream senderStream = (sender == player1) ? stream1 : stream2;
            NetworkStream receiverStream = (sender == player1) ? stream2 : stream1;

            // Check if it's the correct player's turn
            if ((isPlayer1Turn && sender == player1) || (!isPlayer1Turn && sender == player2))
            {
                Console.WriteLine($"LOBBY {LobbyId}: Handling move from player {(sender == player1 ? 1 : 2)}.");

                // Process the move
                string result = $"Player {(isPlayer1Turn ? 1 : 2)} used {move.Substring(5)}!";

                // Send result of the move to both players
                await SendMessage(senderStream, result);  // Send result to the player who made the move
                await SendMessage(receiverStream, result);  // Send result to the opponent

                // Switch turns
                isPlayer1Turn = !isPlayer1Turn;

                // Notify both players about whose turn is next
                string nextTurnMessage = isPlayer1Turn ? "switch_turn:player1" : "switch_turn:player2";
                await SendMessage(stream1, nextTurnMessage);
                await SendMessage(stream2, nextTurnMessage);

            }
            else
            {
                // Inform the sender that it's not their turn
                Console.WriteLine($"LOBBY {LobbyId}: Not player {(sender == player1 ? 1 : 2)}'s turn.");
                await SendMessage(senderStream, "It's not your turn.");
            }
        }

        private async Task SendMessage(NetworkStream stream, string message)
        {
            byte[] response = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(response, 0, response.Length);
        }

        /// <summary>
        /// This helper method sends a Json serialized Pokemon to the client
        /// Made with help from ChatGPT!
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="pokemon"></param>
        private async Task SendPokemon(NetworkStream stream, Pokemon pokemon)
        {
            // Serialize each Pokemon object
            string jsonString = JsonSerializer.Serialize(pokemon);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // Send the length of the message first
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

            // Send the actual JSON data
            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);

            // Wait for data to be read client-side
            await Task.Delay(100);
        }

    }
}
