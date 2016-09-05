using DejarikLibrary;

namespace Assets.Scripts.Monsters
{
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

        public override Node CurrentNode { get; set; }

    }
}
