using System;
using System.Collections.Generic;
using Game.Net;
using Game.Net.NetMessages;

namespace Game.Lobby
{
    public class LobbyClient
    {
        private string name;
        private bool ready;
        private INetClient client;
        private List<ClientData> clientData;

        public Action<ClientData> OnNewClientJoined;
        public Action<ClientData> OnClientReadyChanged;
        public Action<ClientData> OnDisconnected;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool Ready
        {
            get
            {
                return ready;
            }
        }

        public List<ClientData> ClientData
        {
            get
            {
                return clientData;
            }
        }

        public LobbyClient(INetClient client, string name)
        {
            this.client = client;
            this.name = name;
            clientData = new List<ClientData>();
        }

        /// <summary>
        /// This function should be called AFTER the INetClient
        /// has been created AND connected to the server.
        /// After calling this, make sure the INetClient
        /// receives it's token before further using the
        /// LobbyClient.
        /// </summary>
        public void Init()
        {
            client.AddCallback(NetOP.JoinedLobby, OnJoinedLobby);
            client.AddCallback(NetOP.ChangedReady, OnChangedReady);
            client.AddCallback(NetOP.Disconnect, OnDisconnect);
            client.AddCallback(NetOP.ReceiveClients, OnRecieveClients);
        }

        /// <summary>
        /// Use this to un-subscribe network callbacks
        /// </summary>
        public void DeInit()
        {
            client.RemoveCallback(NetOP.JoinedLobby, OnJoinedLobby);
            client.RemoveCallback(NetOP.ChangedReady, OnChangedReady);
            client.RemoveCallback(NetOP.Disconnect, OnDisconnect);
            client.RemoveCallback(NetOP.ReceiveClients, OnRecieveClients);
        }

        /// <summary>
        /// This function sets the local clients ready status
        /// AND sends a NetMsg to the server to update it.
        /// </summary>
        /// <param name="ready"></param>
        public void SetReady(bool ready)
        {
            this.ready = ready;
            Net_ChangedReady m = new Net_ChangedReady() { Token = client.GetToken(), ReadyStatus = ready };
            client.Receive(m);
            client.SendToServer(m);
        }

        public void Join()
        {
            client.SendToServer(new Net_RequestClients());
            Net_JoinedLobby m = new Net_JoinedLobby()
            {
                Client = new ClientData(name, client.GetToken())
            };
            client.Receive(m);
            client.SendToServer(m);
        }

        private void OnJoinedLobby(NetMsg msg)
        {
            Net_JoinedLobby m = (Net_JoinedLobby)msg;
            clientData.Add(m.Client);

            if (OnNewClientJoined != null)
            {
                OnNewClientJoined(m.Client);
            }
        }

        private void OnChangedReady(NetMsg msg)
        {
            Net_ChangedReady m = (Net_ChangedReady)msg;
            foreach (ClientData c in clientData)
            {
                if (c.Token == m.Token)
                {
                    c.Ready = m.ReadyStatus;
                    if (OnClientReadyChanged != null)
                    {
                        OnClientReadyChanged(c);
                    }
                    break;
                }
            }
        }

        private void OnDisconnect(NetMsg msg)
        {
            Net_Disconnect m = (Net_Disconnect)msg;
            ClientData u = null;
            foreach(ClientData c in clientData)
            {
                if (c.Token == m.Token)
                {
                    u = c;
                    clientData.Remove(c);
                    break;
                }
            }

            if (OnDisconnected != null)
            {
                OnDisconnected(u);
            }
        }

        private void OnRecieveClients(NetMsg msg)
        {
            Net_ReceiveClients m = (Net_ReceiveClients)msg;
            if (m.Clients == null)
            {
                Console.WriteLine("WEIRD");
            }
            for (int i = 0; i < m.Clients.Length; i++)
            {
                client.Receive(
                    new Net_JoinedLobby()
                    {
                        Client = m.Clients[i]
                    }
                );
            }
        }
    }
}