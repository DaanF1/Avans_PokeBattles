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

        private List<Pokemon> player1Team;
        private List<Pokemon> player2Team;
        private int player1ActiveIndex = 0;
        private int player2ActiveIndex = 0;

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
            List<Move> venusaurMoves = new List<Move> {
                new Move("Solar Beam", 120, 100, Type.Grass),
                new Move("Take Down", 90, 85, Type.Normal),
                new Move("Razor Leaf", 55, 95, Type.Grass),
                new Move("Tackle", 40, 100, Type.Normal)
            };
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind),
                new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind),
                Type.Grass, 195, 70, venusaurMoves);

            List<Move> charizardMoves = new List<Move> {
                new Move("Scratch", 40, 100, Type.Normal),
                new Move("Inferno", 100, 50, Type.Fire),
                new Move("Slash", 70, 100, Type.Normal),
                new Move("Flamethrower", 90, 100, Type.Fire)
            };
            Pokemon charizard = new Pokemon("Charizard", new Uri(dirPrefix + "/Sprites/aCharizardPreview.png", standardUriKind),
                new Uri(dirPrefix + "/Sprites/aCharizardFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aCharizardAgainst.gif", standardUriKind),
                Type.Fire, 125, 120, charizardMoves);

            List<Move> blastoiseMoves = new List<Move> {
                new Move("Hydro Pump", 110, 80, Type.Water),
                new Move("Aqua Tail", 90, 90, Type.Water),
                new Move("Tackle", 40, 100, Type.Normal),
                new Move("Rapid Spin", 50, 100, Type.Normal)
            };
            Pokemon blastoise = new Pokemon("Blastoise", new Uri(dirPrefix + "/Sprites/aBlastoisePreview.png", standardUriKind),
                new Uri(dirPrefix + "/Sprites/aBlastoiseFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aBlastoiseAgainst.gif", standardUriKind),
                Type.Water, 145, 75, blastoiseMoves);

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
            player1Team = AssignRandomTeam();
            player2Team = AssignRandomTeam();

            Console.WriteLine("LOBBY: Sending 'start-game' signal to both players...");

            await SendMessage(stream1, "start-game:player1");
            await SendMessage(stream2, "start-game:player2");

            // Send teams to both players
            await SendTeam(stream1, player1Team, player2Team, 1);
            await SendTeam(stream2, player2Team, player1Team, 2);

            Console.WriteLine("LOBBY: Start-game messages sent to both players.");

        }

        private List<Pokemon> AssignRandomTeam()
        {
            List<Pokemon> teamOfPlayer = new List<Pokemon>();
            for (int i = 0; i < 6; i++)
            {
                Pokemon randomPokemon = pokemonLister.GetRandomPokemon().DeepCopy();  // Deep copy to ensure unique instance
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

        public async Task HandleMove(string moveMessage, TcpClient sender)
        {
            NetworkStream senderStream = (sender == player1) ? stream1 : stream2;
            NetworkStream receiverStream = (sender == player1) ? stream2 : stream1;
            Pokemon attacker = (sender == player1) ? player1Team[player1ActiveIndex] : player2Team[player2ActiveIndex];
            Pokemon defender = (sender == player1) ? player2Team[player2ActiveIndex] : player1Team[player1ActiveIndex];

            if ((isPlayer1Turn && sender == player1) || (!isPlayer1Turn && sender == player2))
            {
                string moveName = moveMessage.Substring(5);
                Move selectedMove = attacker.PokemonMoves.FirstOrDefault(m => m.MoveName == moveName);

                if (selectedMove != null)
                {
                    int damage = CalculateDamage(attacker, selectedMove, defender);
                    defender.CurrentHealth -= damage;
                    defender.CurrentHealth = Math.Max(defender.CurrentHealth, 0);

                    string result = $"Player {(isPlayer1Turn ? 1 : 2)} used {moveName}! {damage} damage dealt. {defender.Name} has {defender.CurrentHealth} HP left.";
                    await SendMessage(senderStream, result);
                    await SendMessage(receiverStream, result);

                    if (defender.CurrentHealth <= 0)
                    {
                        string faintMessage = $"{defender.Name} fainted! {(sender == player1 ? "player2" : "player1")}";
                        await SendMessage(senderStream, faintMessage);
                        await SendMessage(receiverStream, faintMessage);

                        // Switch to the next Pokémon for the opponent
                        if (sender == player1 && player2ActiveIndex < player2Team.Count - 1)
                        {
                            player2ActiveIndex++;
                        }
                        else if (sender == player2 && player1ActiveIndex < player1Team.Count - 1)
                        {
                            player1ActiveIndex++;
                        }
                        else
                        {
                            string endMessage = $"Game Over! Player {(sender == player1 ? 2 : 1)} wins!";
                            await SendMessage(stream1, endMessage);
                            await SendMessage(stream2, endMessage);
                            return;
                        }
                    }

                    isPlayer1Turn = !isPlayer1Turn;
                    string nextTurnMessage = isPlayer1Turn ? "switch_turn:player1" : "switch_turn:player2";
                    await SendMessage(stream1, nextTurnMessage);
                    await SendMessage(stream2, nextTurnMessage);
                }
                else
                {
                    Console.WriteLine($"Move {moveName} not found for attacker {attacker.Name}");
                }
            }
            else
            {
                await SendMessage(senderStream, "It's not your turn.");
            }
        }

        private int CalculateDamage(Pokemon attacker, Move move, Pokemon defender)
        {
            double typeEffectiveness = GetTypeEffectiveness(move.TypeOfAttack, defender.PokemonType);
            int baseDamage = move.MoveDamage;
            Random random = new Random();
            double randomMultiplier = random.Next(85, 101) / 100.0;
            int damage = (int)(baseDamage * typeEffectiveness * randomMultiplier);
            return Math.Max(damage, 0);
        }

        private double GetTypeEffectiveness(Type attackType, Type defenderType)
        {
            if (attackType == Type.Grass && defenderType == Type.Water) return 2.0;
            if (attackType == Type.Fire && defenderType == Type.Grass) return 2.0;
            if (attackType == Type.Water && defenderType == Type.Fire) return 2.0;
            if (attackType == defenderType) return 0.5;
            return 1.0;
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
