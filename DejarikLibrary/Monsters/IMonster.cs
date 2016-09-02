namespace DejarikLibrary.Monsters
{
    public interface IMonster
    {
        int AttackRating { get; }
        int DefenseRating { get; }
        int MovementRating { get; }
    }
}
