using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace Avans_PokeBattles.Server
{
    public class Lobby
    {
        private TcpClient player1;
        private TcpClient player2;
        private NetworkStream stream1;
        private NetworkStream stream2;

        private bool isPlayer1Turn = true;  // Track whose turn it is

        private List<Pokemon> availablePokemon;  // List of available Pokémon to pick from

        public string LobbyId { get; private set; }
        public bool IsFull { get; private set; }

        public Lobby(string lobbyId)
        {
            LobbyId = lobbyId;
            IsFull = false;
            InitializePokemonList();  // Initialize Pokémon list
        }

        private void InitializePokemonList()
        {
            // Create a small list of available Pokémon (Venusaur, Charizard, Blastoise)
            availablePokemon = new List<Pokemon>()
            {
                new Pokemon("Venusaur", 195),
                new Pokemon("Charizard", 125),
                new Pokemon("Blastoise", 145)
            };
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
                StartGame();  // Start game when the lobby is full
            }
        }

        private async void StartGame()
        {
            // Assign random teams of 6 Pokémon to both players (allowing duplicates)
            List<Pokemon> player1Team = AssignRandomTeam();
            List<Pokemon> player2Team = AssignRandomTeam();

            // Send teams to both players
            await SendTeam(stream1, player1Team, 1);
            await SendTeam(stream2, player2Team, 2);

            // Start the turn-based interaction
            await SendMessage(stream1, "It's your turn. Choose a move.");
            await SendMessage(stream2, "Waiting for Player 1 to choose a move.");

            Task.Run(() => HandleClient(player1, stream1, player2, stream2));
            Task.Run(() => HandleClient(player2, stream2, player1, stream1));
        }

        private List<Pokemon> AssignRandomTeam()
        {
            Random random = new Random();
            List<Pokemon> team = new List<Pokemon>();

            // Select 6 Pokémon randomly (allowing duplicates)
            for (int i = 0; i < 6; i++)
            {
                Pokemon randomPokemon = availablePokemon[random.Next(availablePokemon.Count)];
                team.Add(randomPokemon);  // Allow duplicates by not checking if it's already in the team
            }

            return team;
        }

        private async Task SendTeam(NetworkStream stream, List<Pokemon> team, int playerNumber)
        {
            StringBuilder teamMessage = new StringBuilder();
            teamMessage.Append($"Player {playerNumber} team: ");

            foreach (var pokemon in team)
            {
                teamMessage.Append($"{pokemon.Name} (HP: {pokemon.Health}), ");
            }

            // Send the team data to the player
            await SendMessage(stream, teamMessage.ToString().TrimEnd(','));
        }

        private async Task HandleClient(TcpClient sender, NetworkStream senderStream, TcpClient receiver, NetworkStream receiverStream)
        {
            byte[] buffer = new byte[1500];

            while (sender.Connected && receiver.Connected)
            {
                int bytesRead = await senderStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Message from player: {message}");

                if (IsMoveMessage(message))
                {
                    if (isPlayer1Turn && sender == player1 || !isPlayer1Turn && sender == player2)
                    {
                        await HandleMove(message, senderStream, receiverStream);
                    }
                    else
                    {
                        // Inform the sender that it's not their turn
                        await SendMessage(senderStream, "It's not your turn.");
                    }
                }
                else
                {
                    await SendMessage(senderStream, "Unknown command.");
                }
            }

            // Handle disconnection
            sender.Close();
            receiver.Close();
        }

        private bool IsMoveMessage(string message)
        {
            return message.StartsWith("move:");  // Simplified check for move commands (possible to be expanded upon later)
        }

        private async Task HandleMove(string move, NetworkStream senderStream, NetworkStream receiverStream)
        {
            // Simulate basic move handling logic
            string result = $"Player {(isPlayer1Turn ? 1 : 2)} used {move.Substring(5)}!";

            // Relay the move result to both players
            await SendMessage(senderStream, result);  // To the player who made the move
            await SendMessage(receiverStream, result);  // To the opponent

            // Switch turns
            isPlayer1Turn = !isPlayer1Turn;

            // Notify players about whose turn is next using structured messages
            if (isPlayer1Turn)
            {
                await SendMessage(stream1, "switch_turn:player1");  // Player 1's turn
                await SendMessage(stream2, "switch_turn:player1");  // Notify Player 2 that it's Player 1's turn
            }
            else
            {
                await SendMessage(stream1, "switch_turn:player2");  // Notify Player 1 that it's Player 2's turn
                await SendMessage(stream2, "switch_turn:player2");  // Player 2's turn
            }
        }

        private async Task SendMessage(NetworkStream stream, string message)
        {
            byte[] response = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(response, 0, response.Length);
        }
    }

    // A simple Pokemon class to represent each Pokémon's state in the server
    public class Pokemon
    {
        public string Name { get; }
        public int Health { get; private set; }

        public Pokemon(string name, int health)
        {
            Name = name;
            Health = health;
        }
    }
}
