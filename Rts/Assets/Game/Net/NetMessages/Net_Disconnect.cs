
namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_Disconnect : NetMsg
    {
        public Net_Disconnect()
        {
            OP = NetOP.Disconnect;
        }

        public short Token { get; set; }
    }
}