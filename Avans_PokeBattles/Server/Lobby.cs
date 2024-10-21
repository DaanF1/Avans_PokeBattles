﻿using System.IO;
using System.Net.Sockets;
using System.Text;
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
            Pokemon unown = new Pokemon("Unown", new Uri(dirPrefix + "/Sprites/aUnownPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aUnownFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aUnownAgainst.gif", standardUriKind), unownMoves, 80, 90);
            
            List<Move> venusaurMoves = new List<Move>();
            venusaurMoves.Add(new Move("Solar Beam", 120, 100, Type.Grass));
            venusaurMoves.Add(new Move("Take Down", 90, 85, Type.Normal));
            venusaurMoves.Add(new Move("Razor Leaf", 55, 95, Type.Grass));
            venusaurMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            Pokemon venusaur = new Pokemon("Venusaur", new Uri(dirPrefix + "/Sprites/mVenusaurPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/mVenusaurAgainst.gif", standardUriKind), venusaurMoves, 195, 70);

            List<Move> charizardMoves = new List<Move>();
            charizardMoves.Add(new Move("Scratch", 40, 100, Type.Normal));
            charizardMoves.Add(new Move("Inferno", 100, 50, Type.Fire));
            charizardMoves.Add(new Move("Slash", 70, 100, Type.Normal));
            charizardMoves.Add(new Move("Flamethrower", 90, 100, Type.Fire));
            Pokemon charizard = new Pokemon("Charizard", new Uri(dirPrefix + "/Sprites/aCharizardPreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aCharizardFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aCharizardAgainst.gif", standardUriKind), charizardMoves, 125, 120);

            List<Move> blastoiseMoves = new List<Move>();
            blastoiseMoves.Add(new Move("Hydro Pump", 110, 80, Type.Water));
            blastoiseMoves.Add(new Move("Aqua Tail", 90, 90, Type.Water));
            blastoiseMoves.Add(new Move("Tackle", 40, 100, Type.Normal));
            blastoiseMoves.Add(new Move("Rapid Spin", 50, 100, Type.Normal));
            Pokemon blastoise = new Pokemon("Blastoise", new Uri(dirPrefix + "/Sprites/aBlastoisePreview.png", standardUriKind), new Uri(dirPrefix + "/Sprites/aBlastoiseFor.gif", standardUriKind), new Uri(dirPrefix + "/Sprites/aBlastoiseAgainst.gif", standardUriKind), blastoiseMoves, 145, 75);

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
                Console.WriteLine("Player 1 has joined the lobby.");
            }
            else if (player2 == null)
            {
                player2 = client;
                stream2 = client.GetStream();
                IsFull = true;  // Lobby is full when both players have joined
                Console.WriteLine("Player 2 has joined the lobby.");
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
            // Select 6 Pokémon randomly
            List<Pokemon> teamOfPlayer = new List<Pokemon>();

            for (int i = 0; i < 6; i++)
            {
                Pokemon randomPokemon = pokemonLister.GetRandomPokemon(); // Allow duplicate Pokemon to be in the same team
                teamOfPlayer.Add(randomPokemon);
            }

            return teamOfPlayer;
        }

        private async Task SendTeam(NetworkStream stream, List<Pokemon> team, int playerNumber)
        {
            StringBuilder teamMessage = new StringBuilder();
            teamMessage.Append($"Player {playerNumber} team: ");

            foreach (Pokemon pokemon in team)
            {
                teamMessage.Append($"{SerializeObject(pokemon)}, "); // Add the serialized Pokemon to the message
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
            await stream.FlushAsync();
        }

        /// <summary>
        /// Helper method to serialize an object to a string
        /// StackOverflow: https://stackoverflow.com/questions/2434534/serialize-an-object-to-string 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize); // Serialize the object
                return textWriter.ToString(); // Return Serialized object in string form
            }
        }
    }
}
