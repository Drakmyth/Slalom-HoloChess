using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class AvailableMovesResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int SelectedMonsterTypeId;
        public int[] AvailableMoveNodeIds;
        public int[] AvailableAttackNodeIds;

        public AvailableMovesResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AvailableMovesResponse;
        }

    }
}
