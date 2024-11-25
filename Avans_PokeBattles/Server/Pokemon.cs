using Avans_PokeBattles.Server;
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
        public StatusEffect CurrentStatus { get; set; } = StatusEffect.None;
        public int StatusDuration { get; set; } = 0;

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
            CurrentStatus = StatusEffect.None;
        }


        public void ApplyStatusEffect(StatusEffect effect, int duration)
        {
            if (CurrentStatus == StatusEffect.None || effect == StatusEffect.None)
            {
                CurrentStatus = effect;
                StatusDuration = duration;
            }
        }

        public void HandleStatusEffect()
        {
            if (CurrentStatus == StatusEffect.None) return;

            switch (CurrentStatus)
            {
                case StatusEffect.Burn:
                    int burnDamage = MaxHealth / 16;
                    CurrentHealth -= burnDamage;
                    Console.WriteLine($"{Name} is hurt by its burn and takes {burnDamage} damage!");
                    break;

                case StatusEffect.Poison:
                    int poisonDamage = MaxHealth / 8;
                    CurrentHealth -= poisonDamage;
                    Console.WriteLine($"{Name} is hurt by poison and takes {poisonDamage} damage!");
                    break;

                case StatusEffect.Paralysis:
                    Random random = new();
                    if (random.Next(0, 100) < 25) // 25% chance to be fully paralyzed
                    {
                        Console.WriteLine($"{Name} is paralyzed and cannot move!");
                        return; // Skip turn
                    }
                    break;

                case StatusEffect.Sleep:
                    if (StatusDuration > 0)
                    {
                        Console.WriteLine($"{Name} is asleep and cannot move!");
                        StatusDuration--;
                        return; // Skip turn
                    }
                    break;

                case StatusEffect.Freeze:
                    Console.WriteLine($"{Name} is frozen and cannot move!");
                    Random rng = new();
                    if (rng.Next(0, 100) < 20) // 20% chance to thaw
                    {
                        Console.WriteLine($"{Name} thawed out!");
                        CurrentStatus = StatusEffect.None;
                    }
                    break;
            }

            StatusDuration--;
            if (StatusDuration <= 0)
            {
                Console.WriteLine($"{Name}'s {CurrentStatus} effect has worn off!");
                CurrentStatus = StatusEffect.None;
            }
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
                               PokemonMoves.Select(move =>
                                   new Move(move.MoveName, move.MoveDamage, move.MoveAccuracy, move.TypeOfAttack, move.Effect, move.EffectChance)).ToList())
            {
                CurrentHealth = this.CurrentHealth,
                CurrentStatus = this.CurrentStatus, // Copy current status
                StatusDuration = this.StatusDuration // Copy status duration
            };
        }
    }
}
