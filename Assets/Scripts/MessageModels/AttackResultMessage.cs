using Assets.Scripts.Monsters;
using DejarikLibrary;

namespace Assets.Scripts.MessageModels
{
    public class AttackResultMessage : GameMessage
    {
        public Monster AttackingMonster { get; set; }
        public Monster DefendingMonster { get; set; }
        public AttackResult AttackResult { get; set; }

    }
}
