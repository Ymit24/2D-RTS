using Game.Lobby;

namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_ReceiveClients : NetMsg
    {
        public Net_ReceiveClients()
        {
            OP = NetOP.ReceiveClients;
        }

        public ClientData[] Clients { get; set; }
    }
}