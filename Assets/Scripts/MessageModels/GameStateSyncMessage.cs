using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class GameStateSyncMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;
        public string HostMonsterState;
        public string GuestMonsterState;
        public int SelectedMonsterTypeId;
        public int SelectedAttackNodeId;
        public string MovementPathIds;
        public int DestinationNodeId;
        public string AvailablePushDestinationIds;

        public GameStateSyncMessage()
        {
            MessageTypeId = CustomMessageTypes.GameStateSync;
        }


    }
}
