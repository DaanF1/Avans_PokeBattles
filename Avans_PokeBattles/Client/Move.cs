using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Client
{
    class Move
    {
        private string moveName;
        private int moveDamage;
        private Type typeOfAttack;

        public Move(string name, int attackHealth, int accuracy, Type type)
        {
            this.moveName = name;
            this.moveDamage = attackHealth;
            this.typeOfAttack = type;
        }

        public string MoveName { get { return this.moveName; } }
        public int MoveDamage { get { return this.moveDamage; } }
        public Type TypeOfAttack { get { return this.typeOfAttack; } }
    }
}
