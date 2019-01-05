
namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_RequestToken : NetMsg
    {
        public Net_RequestToken()
        {
            OP = NetOP.RequestToken;
        }
    }
}
