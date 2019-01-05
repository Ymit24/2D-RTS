
namespace Game.Net.NetMessages
{
    [System.Serializable]
    public class Net_AssignToken : NetMsg
    {
        public Net_AssignToken()
        {
            OP = NetOP.AssignToken;
        }

        public short Token { get; set; }
    }
}