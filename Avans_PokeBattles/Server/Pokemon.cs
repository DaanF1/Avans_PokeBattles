using Avans_PokeBattles.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Type = Avans_PokeBattles.Server.Type;

namespace Avans_PokeBattles
{
    [Serializable]
    public class Pokemon
    {
        public string Name { get; set; }
        public string PreviewUri { get; set; }
        public string BattleForUri { get; set; }
        public string BattleAgainstUri { get; set; }
        public Type PokemonType { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Speed { get; set; }
        public List<Move> PokemonMoves { get; set; }

        public Pokemon() { }

        public Pokemon(string name, Uri previewUri, Uri battleForUri, Uri battleAgainstUri, Type type, int maxHealth, int speed, List<Move> moves)
        {
            Name = name;
            PreviewUri = previewUri.ToString();
            BattleForUri = battleForUri.ToString();
            BattleAgainstUri = battleAgainstUri.ToString();
            PokemonType = type;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Speed = speed;
            PokemonMoves = moves;
        }

        /// <summary>
        /// Deserialize a Pokemon sent via TCP
        /// Inspiration from StackOverflow: https://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Pokemon Deserialize(string jsonMessage) //SerialMessage message
        {
            return JsonSerializer.Deserialize<Pokemon>(jsonMessage);
        }

        /// <summary>
        /// Gets a move based on the position of the move in the list
        /// </summary>
        /// <param name="position"></param>
        public Move GetMove(int position)
        {
            if (position >= 0 && position < PokemonMoves.Count)
                return PokemonMoves[position];
            return null;
        }

        public Move GetRandomMove()
        {
            Random random = new Random();
            return PokemonMoves[random.Next(PokemonMoves.Count)];
        }
        public Pokemon DeepCopy()
        {
            return new Pokemon(Name, new Uri(PreviewUri), new Uri(BattleForUri), new Uri(BattleAgainstUri),
                               PokemonType, MaxHealth, Speed,
                               PokemonMoves.Select(move => new Move(move.MoveName, move.MoveDamage, move.MoveAccuracy, move.TypeOfAttack)).ToList())
            {
                CurrentHealth = this.CurrentHealth 
            };
        }
    }
}
