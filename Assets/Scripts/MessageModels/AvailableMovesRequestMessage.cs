using Assets.Scripts.Monsters;

namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterMessage : GameMessage
    {
        public Monster SelectedMonster { get; set; }

    }
}
