namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterRequestMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId { get; set; }

        public override short MessageTypeId
        {
            get { return CustomMessageTypes.SelectMonster; }
        }

    }
}
