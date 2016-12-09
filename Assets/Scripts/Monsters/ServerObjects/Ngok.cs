namespace Assets.Scripts.Monsters.ServerObjects
{
    public class Ngok : Monster
    {
        public override int AttackRating
        {
            get { return 3; }
        }

        public override int DefenseRating
        {
            get { return 8; }
        }

        public override int MovementRating
        {
            get { return 1; }
        }

        public override string Name
        {
            get { return "n'gok"; }
        }

        public override int MonsterTypeId
        {
            get { return MonsterTypes.Ngok; }
        }

    }
}
