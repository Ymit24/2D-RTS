using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game;
using Game.Net;
using Game.Net.NetMessages;
using Game.Lobby;

class Program
{
    static void Main(string[] args)
    {
        new Program();
    }

    private INetServer server;
    private INetClient client;

    private LobbyClient lobbyClient;
    private LobbyServer lobbyServer;

    private string GetName()
    {
        Console.Clear();
        Console.WriteLine("Enter a username:");
        Console.Write(">>");
        return Console.ReadLine();
    }

    private Program()
    {
        bool isServer = false;
        bool isClient = false;
        do
        {
            Console.Clear();
            Console.WriteLine("Start as server or client? (S for server, C for client)");
            Console.Write(">>");
            string input = Console.ReadLine();
            if (input.ToUpper() == "S")
            {
                isServer = true;
            }
            else if (input.ToUpper() == "C")
            {
                isClient = true;
            }
        } while (isServer == false && isClient == false);

        if (isServer)
        {
            server = new SocketServer();
            server.Init();
            server.Start();

            server.AddCallback(NetOP.JoinedLobby, (NetMsg msg, short token) => {
                Net_JoinedLobby m = (Net_JoinedLobby)msg;
                Console.WriteLine("Name {0}, Token {1}, Ready {2}", m.Client.Name, m.Client.Token, m.Client.Ready);
            });

            lobbyServer = new LobbyServer(server);
            lobbyServer.Init();

            ServerView();
        }
        else
        {
            client = new SocketClient();
            client.ConnectToServer();

            client.AddCallback(NetOP.AssignToken, (NetMsg msg) =>
            {
                lobbyClient.Join();
            });

            lobbyClient = new LobbyClient(client, GetName());
            lobbyClient.Init();
            client.RequestToken();


            ClientView();
        }
    }

    private void ServerView()
    {
        List<ClientData> d;
        while (true)
        {
            //Console.Clear();
            d = lobbyServer.ClientData;
            foreach (ClientData c in d)
            {
                Console.WriteLine("{0}: {1}", c.Name, c.Ready ? "Ready" : "Not Ready");
            }
            System.Threading.Thread.Sleep(100);
            server.Process();
        }
    }

    private void ClientView()
    {
        List<ClientData> d;
        while (true)
        {
            //Console.Clear();
            d = lobbyClient.ClientData;
            //Console.WriteLine(lobbyClient.ClientData.Count);
            foreach (ClientData c in d)
            {
                Console.WriteLine("{0}: {1}", c.Name, c.Ready ? "Ready" : "Not Ready");
            }
            if (Console.KeyAvailable)
            {
                if (Console.ReadKey().Key == ConsoleKey.R)
                {
                    lobbyClient.SetReady(!lobbyClient.Ready);
                }
            }
            System.Threading.Thread.Sleep(20);
            client.Process();
        }
    }
}