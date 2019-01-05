
namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_ChangedReady : NetMsg
    {
        public Net_ChangedReady()
        {
            OP = NetOP.ChangedReady;
        }

        public bool ReadyStatus { get; set; }
        public short Token { get; set; }
    }
}