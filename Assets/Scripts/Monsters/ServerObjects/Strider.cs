namespace Assets.Scripts.Monsters.ServerObjects
{
    public class Strider : Monster
    {
        public override int AttackRating
        {
            get { return 2; }
        }

        public override int DefenseRating
        {
            get { return 7; }
        }

        public override int MovementRating
        {
            get { return 3; }
        }

        public override string Name
        {
            get { return "strider"; }
        }

        public override int MonsterTypeId
        {
            get { return MonsterTypes.Strider; }
        }

    }
}
