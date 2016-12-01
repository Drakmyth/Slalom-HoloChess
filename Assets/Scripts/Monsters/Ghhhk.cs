using System;

namespace Assets.Scripts.Monsters
{
    [Serializable]
    public class Ghhhk : Monster
    {
        public override int AttackRating
        {
            get { return 4; }
        }

        public override int DefenseRating
        {
            get { return 3; }
        }

        public override int MovementRating
        {
            get { return 2; }
        }

        public override string Name
        {
            get { return "ghhhk"; }
        }

        public override int MonsterTypeId
        {
            get { return MonsterTypes.Ghhhk; }
        }

    }
}
