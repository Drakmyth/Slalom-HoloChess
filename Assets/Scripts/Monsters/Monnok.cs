namespace Assets.Scripts.Monsters
{
    public class Monnok : Monster
    {
        public override int AttackRating
        {
            get { return 6; }
        }

        public override int DefenseRating
        {
            get { return 5; }
        }

        public override int MovementRating
        {
            get { return 3; }
        }

        public override string Name
        {
            get { return "monnok"; }
        }

        public override int MonsterTypeId
        {
            get { return MonsterTypes.Monnok; }
        }

    }
}
