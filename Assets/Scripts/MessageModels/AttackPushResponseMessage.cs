using System.Collections.Generic;

namespace Assets.Scripts.MessageModels
{
    public class AttackPushResponseMessage : AttackResponseMessage
    {

        public List<int> AvailablePushDestinationIds { get; set; }

        public AttackPushResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackPushResponse;
        }
    }
}
