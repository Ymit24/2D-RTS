namespace Game.Net
{
    public enum NetOP : byte
    {
        None = 0,
        RequestToken = 1,
        AssignToken = 2,
        JoinedLobby = 3,
        ChangedReady = 4,
        StartGame = 5,

        RequestClients = 6,
        ReceiveClients = 7,

        Disconnect = 8,
    }

    [System.Serializable]
    public abstract class NetMsg
    {
        public NetOP OP { set; get; }

        public NetMsg()
        {
            OP = NetOP.None;
        }
    }

}
