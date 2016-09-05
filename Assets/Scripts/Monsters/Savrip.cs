using DejarikLibrary;

namespace Assets.Scripts.Monsters
{
    public class Savrip : Monster
    {
        public override int AttackRating
        {
            get { return 6; }
        }

        public override int DefenseRating
        {
            get { return 6; }
        }

        public override int MovementRating
        {
            get { return 2; }
        }

        public override Node CurrentNode { get; set; }

    }
}
