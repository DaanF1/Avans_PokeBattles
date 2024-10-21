using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Server
{
    public class Move
    {
        // Attributes
        private string moveName;
        private int moveDamage;
        private int accuracy;
        private Type typeOfAttack;

        public Move(string name, int attackHealth, int accuracy, Type type)
        {
            moveName = name;
            moveDamage = attackHealth;
            this.accuracy = accuracy;
            typeOfAttack = type;
        }

        public string MoveName { get { return moveName; } }
        public int MoveDamage { get { return moveDamage; } }
        public int MoveAccuracy { get { return accuracy; } }
        public Type TypeOfAttack { get { return typeOfAttack; } }
    }
}
