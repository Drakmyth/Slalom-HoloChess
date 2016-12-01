using System;

namespace Assets.Scripts.Monsters
{
    [Serializable]
    public class Houjix : Monster
    {
        public override int AttackRating
        {
            get { return 4; }
        }

        public override int DefenseRating
        {
            get { return 4; }
        }

        public override int MovementRating
        {
            get { return 1; }
        }

        public override string Name
        {
            get { return "houjix"; }
        }

        public override int MonsterTypeId
        {
            get { return MonsterTypes.Houjix; }
        }

    }
}
