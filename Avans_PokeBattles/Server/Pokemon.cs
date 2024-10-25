using Avans_PokeBattles.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Type = Avans_PokeBattles.Server.Type;

namespace Avans_PokeBattles
{
    [Serializable]
    public class Pokemon
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string PreviewUri { get; set; }

        [XmlElement]
        public string BattleForUri { get; set; }

        [XmlElement]
        public string BattleAgainstUri { get; set; }

        [XmlElement]
        public Type PokemonType { get; set; }

        [XmlElement]
        public int MaxHealth { get; set; }

        [XmlElement]
        public int CurrentHealth { get; set; }

        [XmlElement]
        public int Speed { get; set; }

        [XmlArray("Moves")]
        [XmlArrayItem("Move")]
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
    }
}
