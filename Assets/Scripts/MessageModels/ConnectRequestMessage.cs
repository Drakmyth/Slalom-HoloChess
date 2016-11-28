namespace Assets.Scripts.MessageModels
{
    public class ConnectRequestMessage : GameMessage
    {

        public string ClientName { get; set; }
        public bool IsHost  { get; set; }

    }
}
