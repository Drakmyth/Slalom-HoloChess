using System;

namespace Assets.Scripts.Monsters
{
    [Serializable]
    public class Klorslug : Monster
    {
        public override int AttackRating
        {
            get { return 7; }
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
            get { return "k'lor'slug"; }
        }

        public override int MonsterTypeId
        {
            get { return MonsterTypes.Klorslug; }
        }

    }
}
