namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_JoinedLobby : NetMsg
    {
        public Net_JoinedLobby()
        {
            OP = NetOP.JoinedLobby;
        }

        public short Token { get; set; }
        public string Name { get; set; }
    }
}