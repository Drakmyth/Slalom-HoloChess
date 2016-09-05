using DejarikLibrary;

namespace Assets.Scripts.Monsters
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

        public override Node CurrentNode { get; set; }

    }
}
