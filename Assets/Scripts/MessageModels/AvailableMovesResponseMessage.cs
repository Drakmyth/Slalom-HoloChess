namespace Assets.Scripts.MessageModels
{
    public class AvailableMovesResponseMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId;
        public int[] AvailableMoveNodeIds;
        public int[] AvailableAttackNodeIds;

        public AvailableMovesResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AvailableMovesResponse;
        }

    }
}
