using System;

namespace Game.Net
{
    public interface INetClient
    {
        // short GetToken();
        void ConnectToServer();
        void Disconnect();
        void SendToServer(NetMsg msg);
        void Receive(NetMsg msg);
        void Process();
    }
}
