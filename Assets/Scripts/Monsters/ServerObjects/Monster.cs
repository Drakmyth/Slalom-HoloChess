using DejarikLibrary;

namespace Assets.Scripts.Monsters.ServerObjects
{
    public abstract class Monster
    {
        public abstract int AttackRating { get; }
        public abstract int DefenseRating { get; }
        public abstract int MovementRating { get; }
        public abstract string Name { get; }
        public abstract int MonsterTypeId { get; }

        public Node CurrentNode { get; set; }

        public override bool Equals(object o)
        {
            return Equals(o as Monster);
        }

        public bool Equals(Monster monster)
        {
            return monster.AttackRating == AttackRating && monster.DefenseRating == DefenseRating &&
                   monster.MovementRating == MovementRating;
        }

        public override int GetHashCode()
        {
            return AttackRating.GetHashCode() + DefenseRating.GetHashCode() + MovementRating.GetHashCode();
        }
    }
}
