namespace Assets.Scripts.MessageModels
{
    public class AvailableMovesResponseMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId;
        public int[] AvailableMoveNodeIds;
        public int[] AvailableAttackNodeIds;

        public override short MessageTypeId
        {
            get { return CustomMessageTypes.AvailableMovesResponse; }
        }

    }
}
