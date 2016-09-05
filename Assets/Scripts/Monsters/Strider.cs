using DejarikLibrary;

namespace Assets.Scripts.Monsters
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

        public override Node CurrentNode { get; set; }

    }
}
