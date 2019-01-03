using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class UnityClient : Game.Net.INetClient
{
    public void ConnectToServer() { }
    public void SendToServer(Game.Net.NetMsg msg) { }
    public void Receive(Game.Net.NetMsg msg) { }
    public void Process() { }
    public void Disconnect() { }
}