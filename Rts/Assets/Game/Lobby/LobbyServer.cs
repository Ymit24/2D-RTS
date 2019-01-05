using System;
using System.Collections.Generic;
using Game.Net;
using Game.Net.NetMessages;

namespace Game.Lobby
{
    public class LobbyServer
    {
        private INetServer server;
        private List<ClientData> clientData;

        public Action<ClientData> OnClientAdded;
        public Action<ClientData> OnClientChangedReady;
        public Action<ClientData> OnClientDisconnected;

        public List<ClientData> ClientData
        {
            get
            {
                return clientData;
            }
        }

        public LobbyServer(INetServer server)
        {
            this.server = server;
            clientData = new List<ClientData>();
        }

        /// <summary>
        /// This function should be called AFTER the INetServer
        /// has been created.
        /// </summary>
        public void Init()
        {
            server.AddCallback(NetOP.JoinedLobby, OnClientJoinedLobby);
            server.AddCallback(NetOP.ChangedReady, OnClientChangedReadyStatus);
            server.AddCallback(NetOP.RequestClients, OnClientRequestClients);
            server.AddCallback(NetOP.Disconnect, OnClientDisconnect);
        }

        public void DeInit()
        {
            server.RemoveCallback(NetOP.JoinedLobby, OnClientJoinedLobby);
            server.RemoveCallback(NetOP.ChangedReady, OnClientChangedReadyStatus);
            server.RemoveCallback(NetOP.RequestClients, OnClientRequestClients);
            server.RemoveCallback(NetOP.Disconnect, OnClientDisconnect);
        }

        private void OnClientJoinedLobby(NetMsg msg, short token)
        {
            Net_JoinedLobby m = (Net_JoinedLobby)msg;
            ClientData c = new ClientData(m.Name, m.Token);
            clientData.Add(c);
            server.BroadcastEx(m, token);
            if (OnClientAdded != null)
            {
                OnClientAdded(c);
            }
        }

        private void OnClientChangedReadyStatus(NetMsg msg, short token)
        {
            Net_ChangedReady m = (Net_ChangedReady)msg;
            ClientData u = null;
            foreach (ClientData c in clientData)
            {
                if (c.Token == token)
                {
                    c.Ready = m.ReadyStatus;
                    u = c;
                }
            }
            server.BroadcastEx(m, token);

            if (OnClientChangedReady != null)
            {
                OnClientChangedReady(u);
            }
        }

        private void OnClientRequestClients(NetMsg msg, short token)
        {
            Console.WriteLine("requested client");
            List<ClientData> clientsEx = new List<ClientData>();
            for (int i = 0; i < clientData.Count; i++)
            {
                ClientData c = clientData[i];
                if (c.Token == token)
                {
                    continue;
                }
                clientsEx.Add(c);
            }
            server.SendToClient(new Net_ReceiveClients() { Clients = clientsEx.ToArray() }, token);
        }

        private void OnClientDisconnect(NetMsg msg, short token)
        {
            Net_Disconnect m = (Net_Disconnect)msg;
            ClientData u = null;
            for (int i = 0; i < clientData.Count; i++)
            {
                if (clientData[i].Token == token)
                {
                    u = clientData[i];
                    clientData.RemoveAt(i);
                    break;
                }
            }
            server.BroadcastEx(m, token);

            if (OnClientDisconnected != null)
            {
                OnClientDisconnected(u);
            }
        }
    }
}