using Assets.Scripts.Monsters;

namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterStateMessage : GameStateMessage
    {
        public Monster SelectedMonster { get; set; }

    }
}
