using DejarikLibrary;

namespace Assets.Scripts.Monsters
{
    public interface IMonster
    {
        int AttackRating { get; }
        int DefenseRating { get; }
        int MovementRating { get; }

        Node CurrentNode { get; set; }
    }
}
