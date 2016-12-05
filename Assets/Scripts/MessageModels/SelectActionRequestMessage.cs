﻿using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class SelectActionRequestMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int SelectedNodeId { get; set; }

        public SelectActionRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectActionRequest;
        }
    }
}
