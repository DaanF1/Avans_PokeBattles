namespace Avans_PokeBattles.Server
{
    [Serializable]
    public class Move
    {
        public string MoveName { get; set; }
        public int MoveDamage { get; set; }
        public int MoveAccuracy { get; set; }
        public Type TypeOfAttack { get; set; }
        public StatusEffect Effect { get; set; } = StatusEffect.None;
        public int EffectChance { get; set; } = 0;
        public int HealingAmount { get; set; } = 0;

        public Move() { }

        public Move(string name, int attackHealth, int accuracy, Type type, StatusEffect effect, int effectChance, int healingAmount)
        {
            MoveName = name;
            MoveDamage = attackHealth;
            MoveAccuracy = accuracy;
            TypeOfAttack = type;
            Effect = effect;
            EffectChance = effectChance;
            HealingAmount = healingAmount;
        }
    }

}
