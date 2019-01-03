using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    private Game.Net.INetServer server;
    private Game.Net.INetClient client;
    private bool isHost;

    public void ConnectToServer() {
        isHost = false;
        client = new Game.Net.SocketClient();
        client.ConnectToServer();
    }

    public void HostAsServer()
    {
        isHost = true;
        server = new Game.Net.SocketServer();
        server.Init();
        server.Start();
    }

    public void SendDummyTokenToServer()
    {
        if (client != null)
        {
            Game.Net.NetMessages.Net_AssignToken nat = new Game.Net.NetMessages.Net_AssignToken();
            nat.Token = 213;
            client.SendToServer(nat);
        }
    }

    public void SendDummyTokenToClient()
    {
        if (server != null)
        {
            Game.Net.NetMessages.Net_AssignToken nat = new Game.Net.NetMessages.Net_AssignToken();
            nat.Token = 648;
            server.Broadcast(nat);
        }
    }

    public void Update()
    {
        if (client != null)
        {
            client.Process();
        }

        if (server != null)
        {
            server.Process();
        }
    }
}
