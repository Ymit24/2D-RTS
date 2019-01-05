using Game.Lobby;
namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_JoinedLobby : NetMsg
    {
        public Net_JoinedLobby()
        {
            OP = NetOP.JoinedLobby;
        }
        public ClientData Client;
    }
}