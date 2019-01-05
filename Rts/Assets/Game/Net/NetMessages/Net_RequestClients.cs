
namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_RequestClients : NetMsg
    {
        public Net_RequestClients()
        {
            OP = NetOP.RequestClients;
        }
    }
}