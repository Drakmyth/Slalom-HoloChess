namespace Assets.Scripts.MessageModels
{
    public static class CustomMessageTypes
    {
        // Must be above 47
        public static readonly short GameState = 100;
        public static readonly short GameStart = 101;
        public static readonly short ConnectRequest = 102;
        public static readonly short MoveRequest = 103;
        public static readonly short MoveResponse = 104;
        public static readonly short AvailableMovesRequest = 105;
        public static readonly short AvailableMovesResponse = 106;
        public static readonly short AttackRequest = 107;
        public static readonly short AttackKillResponse = 108;
        public static readonly short AttackPushResponse = 109;
        public static readonly short SelectMonsterRequest = 110;
        public static readonly short SelectMonsterResponse = 111;
        public static readonly short SelectActionRequest = 112;
        public static readonly short SelectMoveActionResponse = 113;
        public static readonly short SelectAttackActionResponse = 114;
        public static readonly short AvailableMonstersResponse = 115;
        public static readonly short PushDestinationRequest = 116;
        public static readonly short PushDestinationResponse = 117;
        public static readonly short StateAck = 118;
        public static readonly short GameEnd = 119;
        public static readonly short GameStateSync = 120;
        public static readonly short PassAction = 121;
    }
}
