using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Avans_PokeBattles.Server
{
    [Serializable]
    public class Move
    {
        // Attributes
        public string MoveName { get; set; }
        public int MoveDamage { get; set; }
        public int MoveAccuracy { get; set; }
        public Type TypeOfAttack { get; set; }

        public Move() { }
        public Move(string name, int attackHealth, int accuracy, Type type)
        {
            MoveName = name;
            MoveDamage = attackHealth;
            MoveAccuracy = accuracy;
            TypeOfAttack = type;
        }

    }
}
