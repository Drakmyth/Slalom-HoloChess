using DejarikLibrary;

namespace Assets.Scripts.Monsters
{
    public class Molator : Monster
    {
        public override int AttackRating
        {
            get { return 8; }
        }

        public override int DefenseRating
        {
            get { return 2; }
        }

        public override int MovementRating
        {
            get { return 2; }
        }

        public override Node CurrentNode { get; set; }

    }
}
