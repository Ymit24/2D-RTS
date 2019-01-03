using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Net
{
    public interface INetServer
    {
        void Init();
        void Start();
        void Stop();
        void Process();
        void Broadcast(NetMsg msg);
        void Receive(NetMsg msg);
    }
}
