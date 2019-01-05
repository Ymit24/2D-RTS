using System;
using System.Collections.Generic;

namespace Game.Net
{
    public interface INetServer
    {
        void Init();
        void Start();
        void Stop();
        void Process();
        void Broadcast(NetMsg msg);
        void BroadcastEx(NetMsg msg, short token);
        void SendToClient(NetMsg msg, short token);
        void AddCallback(NetOP op, Action<NetMsg, short> action);
        void RemoveCallback(NetOP op, Action<NetMsg, short> action);

    }
}
