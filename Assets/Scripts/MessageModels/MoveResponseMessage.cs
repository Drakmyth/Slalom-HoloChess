namespace Assets.Scripts.MessageModels
{
    public class MoveResponseMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId { get; set; }
        public string NodePath { get; set; } //TODO: Net Serialize

        public override short MessageTypeId
        {
            get { return CustomMessageTypes.MoveResponse; }
        }

    }
}
