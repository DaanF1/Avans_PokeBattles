using System.IO;
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

        private List<Pokemon> player1Team;
        private List<Pokemon> player2Team;
        private int player1ActiveIndex = 0;
        private int player2ActiveIndex = 0;

        private static readonly ProfileManager profileManager = ProfileManager.Instance;
        public string LobbyId { get; private set; }
        public bool IsFull { get; private set; }

        public Lobby(string lobbyId)
        {
            LobbyId = lobbyId;
            IsFull = false;
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
            Console.WriteLine("LOBBY: Sending 'start-game' signal to both players...");

            await SendMessage(stream1, "start-game:player1");
            await SendMessage(stream2, "start-game:player2");

            await SendNames(stream1);
            await SendNames(stream2);

            Console.WriteLine("LOBBY: Start-game messages sent to both players.");
        }

        public bool HasClient(TcpClient client)
        {
            return client == player1 || client == player2;
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

                await Task.Delay(1000);

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

                await Task.Delay(1000);

                // Sending Player 1 name:
                teamMessage.Append($"Player 2 name:");
                await SendMessage(stream, teamMessage.ToString() + namePlayer1.ToString() + "\n");
                teamMessage.Clear();
            }
        }

        public async Task HandleMove(string message, TcpClient sender)
        {
            NetworkStream senderStream = (sender == player1) ? stream1 : stream2;
            NetworkStream receiverStream = (sender == player1) ? stream2 : stream1;

            var player1Profile = profileManager.GetProfile(namePlayer1);
            var player2Profile = profileManager.GetProfile(namePlayer2);

            player1Team = player1Profile.GetTeam();
            player2Team = player2Profile.GetTeam();

            Pokemon attacker = (sender == player1) ? player1Team[player1ActiveIndex] : player2Team[player2ActiveIndex];
            Pokemon defender = (sender == player1) ? player2Team[player2ActiveIndex] : player1Team[player1ActiveIndex];

            // Check if it's the sender's turn
            if ((isPlayer1Turn && sender == player1) || (!isPlayer1Turn && sender == player2))
            {
                // Check status effect of the attacker
                if (!attacker.HandleStatusEffect())
                {
                    // Notify both players that the turn is skipped due to status effect
                    string statusMessage = $"{attacker.Name} is affected by {attacker.CurrentStatus} and cannot move!";
                    await SendMessage(senderStream, statusMessage);
                    await SendMessage(receiverStream, statusMessage);

                    // Switch turn
                    isPlayer1Turn = !isPlayer1Turn;
                    string nextTurnMessage = isPlayer1Turn ? "switch_turn:player1" : "switch_turn:player2";
                    await SendMessage(stream1, nextTurnMessage);
                    await SendMessage(stream2, nextTurnMessage);
                    return;
                }

                // Process move logic
                string moveName = message.Substring(5);
                Move selectedMove = attacker.PokemonMoves.FirstOrDefault(m => m.MoveName == moveName);

                if (selectedMove != null)
                {
                    if (selectedMove.HealingAmount > 0) // Handle healing moves
                    {
                        int healAmount = selectedMove.HealingAmount;
                        attacker.CurrentHealth += healAmount;
                        attacker.CurrentHealth = Math.Min(attacker.CurrentHealth, attacker.MaxHealth); // Ensure health does not exceed max

                        string healMessage = $"{attacker.Name} used {moveName} and healed for {healAmount} HP!";
                        await SendMessage(senderStream, healMessage);
                        await SendMessage(receiverStream, healMessage);
                    }
                    else // Handle damaging moves
                    {
                        int damage = CalculateDamage(attacker, selectedMove, defender);
                        defender.CurrentHealth -= damage;
                        defender.CurrentHealth = Math.Max(defender.CurrentHealth, 0);

                        string result = $"Player {(isPlayer1Turn ? 1 : 2)} used {moveName}! {damage} damage dealt. {defender.Name} has {defender.CurrentHealth} HP left.";
                        await SendMessage(senderStream, result);
                        await SendMessage(receiverStream, result);

                        if (selectedMove.Effect != StatusEffect.None && new Random().Next(0, 100) < selectedMove.EffectChance)
                        {
                            defender.ApplyStatusEffect(selectedMove.Effect, 2); // Example duration of 2 turns
                            string effectMessage = $"{defender.Name} is now {selectedMove.Effect}!";
                            await SendMessage(senderStream, effectMessage);
                            await SendMessage(receiverStream, effectMessage);
                        }

                        if (defender.CurrentHealth <= 0)
                        {
                            string faintMessage = $"{defender.Name} fainted!";
                            await SendMessage(senderStream, faintMessage);
                            await SendMessage(receiverStream, faintMessage);

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
                                string endMessage;
                                if (player1Team.Count == 0)
                                {
                                    profileManager.GetProfile(namePlayer2).IncrementWins();
                                    profileManager.GetProfile(namePlayer1).IncrementLosses(); // Lol L verliezer
                                    endMessage = $"Game Over! Player 2 ({namePlayer2}) wins";
                                }
                                else
                                {
                                    profileManager.GetProfile(namePlayer1).IncrementWins();
                                    profileManager.GetProfile(namePlayer2).IncrementLosses(); // Lol L verliezer
                                    endMessage = $"Game Over! Player 1 ({namePlayer1}) wins";
                                }

                                await SendMessage(stream1, endMessage);
                                await SendMessage(stream2, endMessage);
                                return;
                            }
                        }
                    }

                    // Switch turn
                    isPlayer1Turn = !isPlayer1Turn;
                    string nextTurnMessage = isPlayer1Turn ? "switch_turn:player1" : "switch_turn:player2";
                    await SendMessage(stream1, nextTurnMessage);
                    await SendMessage(stream2, nextTurnMessage);
                }
            }
            else
            {
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
            if (attackType == Type.Electric && defenderType == Type.Water) return 2.0;  // Electric is super effective against Water
            if (attackType == Type.Normal && defenderType == Type.Ghost) return 2.0;  // Normal is super effective against Ghost
            if (attackType == Type.Ghost && defenderType == Type.Ghost) return 2.0;  // Ghost is super effective against Ghost

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

    }
}
