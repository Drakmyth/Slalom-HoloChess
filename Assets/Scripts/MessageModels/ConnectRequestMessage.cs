﻿namespace Assets.Scripts.MessageModels
{
    public class ConnectRequestMessage : GameStateMessage
    {

        public string ClientName { get; set; }
        public bool IsHost  { get; set; }

        public override short MessageTypeId
        {
            get { return CustomMessageTypes.ConnectRequest; }
        }

    }
}
