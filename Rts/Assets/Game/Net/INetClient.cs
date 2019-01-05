using System;
using System.Collections.Generic;
namespace Game.Net
{
    public interface INetClient
    {
        short GetToken();
        void ConnectToServer();
        void Disconnect();
        void SendToServer(NetMsg msg);
        void Receive(NetMsg msg);
        void Process();
        void AddCallback(NetOP op, Action<NetMsg> action);
        void RemoveCallback(NetOP op, Action<NetMsg> action);
        void RequestToken();
        bool HasToken();
    }
}
