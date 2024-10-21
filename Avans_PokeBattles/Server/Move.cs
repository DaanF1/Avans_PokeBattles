using System;
using System.Xml.Serialization;

namespace Avans_PokeBattles.Server
{
    [Serializable]
    public class Move
    {
        // Public properties for serialization
        [XmlElement]
        public string MoveName { get; set; }

        [XmlElement]
        public int MoveDamage { get; set; }

        [XmlElement]
        public int MoveAccuracy { get; set; }

        [XmlElement]
        public Type TypeOfAttack { get; set; }

        // Default constructor required for serialization
        public Move() { }

        // Constructor to initialize a Move
        public Move(string name, int attackHealth, int accuracy, Type type)
        {
            MoveName = name;
            MoveDamage = attackHealth;
            MoveAccuracy = accuracy;
            TypeOfAttack = type;
        }
    }
}
