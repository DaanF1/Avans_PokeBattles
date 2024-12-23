using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Avans_PokeBattles.Server
{
    public class Lobby
    {
        private TcpClient player1;
        private string namePlayer1;
        private TcpClient player2;
        private string namePlayer2;
        private NetworkStream stream1;
        private NetworkStream stream2;

        private bool isPlayer1Turn = true;  // Track whose turn it is
        private readonly PokemonLister pokemonLister = new();  // List of available Pokémon to pick from
        public string dirPrefix = AppDomain.CurrentDomain.BaseDirectory; // Directory prefix for files
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
            List<Move> unownMoves = [
                new Move("Solar Beam", 120, 100, Type.Grass),
                new Move("Inferno", 100, 50, Type.Fire),
                new Move("Hidden Power", 90, 90, Type.Normal),
                new Move("Hydro Pump", 110, 80, Type.Water)
            ];
            Pokemon unown = new("Unown", new Uri(dirPrefix + "Sprites/aUnownPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aUnownFor.gif", standardUriKind), new Uri(dirPrefix + "Sprites/aUnownAgainst.gif", standardUriKind),
                Type.Normal, 80, 110, unownMoves);

            List<Move> venusaurMoves = [
                new Move("Solar Beam", 120, 100, Type.Grass),
                new Move("Take Down", 90, 85, Type.Normal),
                new Move("Razor Leaf", 55, 95, Type.Grass),
                new Move("Tackle", 40, 100, Type.Normal)
            ];
            Pokemon venusaur = new("Venusaur", new Uri(dirPrefix + "Sprites/aVenusaurPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aVenusaurFor.gif", standardUriKind), new Uri(dirPrefix + "Sprites/aVenusaurAgainst.gif", standardUriKind),
                Type.Grass, 195, 70, venusaurMoves);

            List<Move> charizardMoves = [
                new Move("Scratch", 40, 100, Type.Normal),
                new Move("Inferno", 100, 50, Type.Fire),
                new Move("Slash", 70, 100, Type.Normal),
                new Move("Flamethrower", 90, 100, Type.Fire)
            ];
            Pokemon charizard = new("Charizard", new Uri(dirPrefix + "Sprites/aCharizardPreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aCharizardFor.gif", standardUriKind), new Uri(dirPrefix + "Sprites/aCharizardAgainst.gif", standardUriKind),
                Type.Fire, 125, 120, charizardMoves);

            List<Move> blastoiseMoves = [
                new Move("Hydro Pump", 110, 80, Type.Water),
                new Move("Aqua Tail", 90, 90, Type.Water),
                new Move("Tackle", 40, 100, Type.Normal),
                new Move("Rapid Spin", 50, 100, Type.Normal)
            ];
            Pokemon blastoise = new("Blastoise", new Uri(dirPrefix + "Sprites/aBlastoisePreview.png", standardUriKind),
                new Uri(dirPrefix + "Sprites/aBlastoiseFor.gif", standardUriKind), new Uri(dirPrefix + "Sprites/aBlastoiseAgainst.gif", standardUriKind),
                Type.Water, 145, 75, blastoiseMoves);

            pokemonLister.AddAllPokemon([unown, venusaur, charizard, blastoise]);
        }

        public void AddPlayer(TcpClient client, string playerName)
        {
            if (player1 == null)
            {
                player1 = client;
                namePlayer1 = playerName;
                stream1 = client.GetStream();
            }
            else if (player2 == null)
            {
                player2 = client;
                namePlayer2 = playerName;
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

            await SendNames(stream1);
            await SendNames(stream2);

            Console.WriteLine("LOBBY: Start-game messages sent to both players.");
        }

        private List<Pokemon> AssignRandomTeam()
        {
            List<Pokemon> teamOfPlayer = [];
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

        private static async Task SendTeam(NetworkStream stream, List<Pokemon> playerTeam, List<Pokemon> opponentTeam, int playerNumber)
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

        private async Task SendNames(NetworkStream stream)
        {
            StringBuilder teamMessage = new();

            if (stream == stream1)
            {
                // Sending Player 1 name:
                teamMessage.Append($"Player 1 name:");
                await SendMessage(stream, teamMessage.ToString() + namePlayer1.ToString() + "\n");
                teamMessage.Clear();

                // Sending Player 2 name:
                teamMessage.Append($"Player 2 name:");
                await SendMessage(stream, teamMessage.ToString() + namePlayer2.ToString() + "\n");
                teamMessage.Clear();
            }
            else // Inverse player numbers
            {
                // Sending Player 2 name:
                teamMessage.Append($"Player 1 name:");
                await SendMessage(stream, teamMessage.ToString() + namePlayer2.ToString() + "\n");
                teamMessage.Clear();

                // Sending Player 1 name:
                teamMessage.Append($"Player 2 name:");
                await SendMessage(stream, teamMessage.ToString() + namePlayer1.ToString() + "\n");
                teamMessage.Clear();
            }
        }

        public async Task HandleMove(string message, TcpClient sender)
        {
            // Determine the sender's stream and the opponent's stream based on the sender
            NetworkStream senderStream = (sender == player1) ? stream1 : stream2;
            NetworkStream receiverStream = (sender == player1) ? stream2 : stream1;

            // Determine which Pokémon are the attacker and defender based on whose turn it is
            Pokemon attacker = (sender == player1) ? player1Team[player1ActiveIndex] : player2Team[player2ActiveIndex];
            Pokemon defender = (sender == player1) ? player2Team[player2ActiveIndex] : player1Team[player1ActiveIndex];

            // Check if it is the correct player’s turn
            if ((isPlayer1Turn && sender == player1) || (!isPlayer1Turn && sender == player2))
            {
                // Extract the move name from the message
                string moveName = message.Substring(5);  // Assumes message format like "move:MoveName"

                // Find the selected move in the attacker’s move list
                Move selectedMove = attacker.PokemonMoves.FirstOrDefault(m => m.MoveName == moveName);

                if (selectedMove != null)
                {
                    // Calculate the damage dealt by the selected move and adjust the defender's health
                    int damage = CalculateDamage(attacker, selectedMove, defender);
                    defender.CurrentHealth -= damage;
                    defender.CurrentHealth = Math.Max(defender.CurrentHealth, 0);  // Prevent health from going below zero

                    // Send the result message to both players, indicating the move used and the resulting HP
                    string result = $"Player {(isPlayer1Turn ? 1 : 2)} used {moveName}! {damage} damage dealt. {defender.Name} has {defender.CurrentHealth} HP left.";
                    await SendMessage(senderStream, result);
                    await SendMessage(receiverStream, result);

                    // If the defender’s health reaches zero, send a fainted message
                    if (defender.CurrentHealth <= 0)
                    {
                        string faintMessage = $"{defender.Name} fainted! {(sender == player1 ? "player2" : "player1")}";
                        await SendMessage(senderStream, faintMessage);
                        await SendMessage(receiverStream, faintMessage);

                        // Check if there are additional Pokémon available for the opponent and switch to the next
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
                            // If no more Pokémon are available, end the game and announce the winner
                            string endMessage = $"Game Over! Player {(sender == player1 ? 2 : 1)} wins!";
                            await SendMessage(stream1, endMessage);
                            await SendMessage(stream2, endMessage);
                            return;
                        }
                    }

                    // Toggle the turn to the other player and send a message indicating whose turn it is
                    isPlayer1Turn = !isPlayer1Turn;
                    string nextTurnMessage = isPlayer1Turn ? "switch_turn:player1" : "switch_turn:player2";
                    await SendMessage(stream1, nextTurnMessage);
                    await SendMessage(stream2, nextTurnMessage);
                }
                else if (message.StartsWith("chat:"))
                {
                    // If the message is a chat message, relay it to the other player
                    await SendMessage(receiverStream, message);
                }
                else
                {
                    // Log an error if the move is not found for the attacker
                    Console.WriteLine($"Move {moveName} not found for attacker {attacker.Name}");
                }
            }
            else
            {
                // If it’s not the sender's turn, inform them
                await SendMessage(senderStream, "It's not your turn.");
            }
        }

        // Asynchronously handles chat messages between players by routing the message from one player to the other.
        public async Task HandleChat(string message, TcpClient sender)
        {
            // If player1 is the sender, send the chat message to player2
            if (sender == player1)
            {
                await SendMessage(player2.GetStream(), message);
            }
            // If player2 is the sender, send the chat message to player1
            else if (sender == player2)
            {
                await SendMessage(player1.GetStream(), message);
            }
        }

        // Calculates the damage dealt by a Pokémon's move, taking into account type effectiveness and a random multiplier.
        private int CalculateDamage(Pokemon attacker, Move move, Pokemon defender)
        {
            // Get type effectiveness multiplier based on the types of the attack move and the defender Pokémon
            double typeEffectiveness = GetTypeEffectiveness(move.TypeOfAttack, defender.PokemonType);

            // Base damage of the move
            int baseDamage = move.MoveDamage;

            // Generate a random multiplier between 0.85 and 1.00 to add some variability to damage
            Random random = new();
            double randomMultiplier = random.Next(85, 101) / 100.0;

            // Calculate final damage by applying type effectiveness and random multiplier
            int damage = (int)(baseDamage * typeEffectiveness * randomMultiplier);

            // Ensure that damage is not negative and return it
            return Math.Max(damage, 0);
        }


        // Determines the type effectiveness multiplier for a Pokémon's attack based on the attack type and the defender's type.
        private static double GetTypeEffectiveness(Type attackType, Type defenderType)
        {
            // Define conditions where the attacking type has an advantage over the defending type
            if (attackType == Type.Grass && defenderType == Type.Water) return 2.0; // Grass is super effective against Water
            if (attackType == Type.Fire && defenderType == Type.Grass) return 2.0;  // Fire is super effective against Grass
            if (attackType == Type.Water && defenderType == Type.Fire) return 2.0;  // Water is super effective against Fire

            // If the attacking type and defending type are the same, reduce damage
            if (attackType == defenderType) return 0.5;

            // Return neutral effectiveness (1.0) if none of the above conditions are met
            return 1.0;
        }


        // Sends a UTF-8 encoded message asynchronously over the specified network stream.
        private static async Task SendMessage(NetworkStream stream, string message)
        {
            // Encode the message into bytes and write it to the stream asynchronously
            byte[] response = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(response);
        }

        /// <summary>
        /// This helper method sends a Json serialized Pokemon to the client
        /// Made with help from ChatGPT!
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="pokemon"></param>
        private static async Task SendPokemon(NetworkStream stream, Pokemon pokemon)
        {
            // Serialize each Pokemon object
            string jsonString = JsonSerializer.Serialize(pokemon);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // Send the length of the message first
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            await stream.WriteAsync(lengthBytes);

            // Send the actual JSON data
            await stream.WriteAsync(jsonBytes);

            // Wait for data to be read client-side
            await Task.Delay(50);
        }

    }
}
