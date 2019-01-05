using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Game.Net
{
    using NetMessages;
    using System.Reflection;

    public class SocketServer : INetServer
    {
        private TcpListener listener;
        public void Init()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8090);
            OnNetMsgReceived = new Dictionary<NetOP, Action<NetMsg, short>>();
        }

        public class ClientConnection
        {
            public short Token;
            private TcpClient connection;

            public TcpClient Connection
            {
                get
                {
                    return connection;
                }
            }

            public ClientConnection(TcpClient connection)
            {
                this.connection = connection;
            }
        }

        private List<ClientConnection> clients;
        private Thread listeningThread;

        private static object _locker = new object();
        private short current_token = 0;

        public Dictionary<NetOP, Action<NetMsg, short>> OnNetMsgReceived { get; set; }

        public void Start()
        {
            Log("Started listening..");
            clients = new List<ClientConnection>();
            listener.Start();
            listeningThread = new Thread(() =>
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (_locker)
                    {
                        Log("Client added!");
                        clients.Add(new ClientConnection(client));
                    }
                }
            });
            listeningThread.Start();
        }

        public void Stop()
        {
            Log("Stopping..");
            if (listeningThread != null)
            {
                listeningThread.Abort();
            }
            if (clients != null)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].Connection.Close();
                }
                clients.Clear();
            }
        }

        public void Process()
        {
            lock (_locker)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].Connection.Client.Poll(1000, SelectMode.SelectRead) &
                        (clients[i].Connection.Client.Available == 0))
                    {
                        Log("Client disconnected");

                        Receive(new Net_Disconnect() { Token = clients[i].Token }, clients[i]);

                        clients.RemoveAt(i);
                        i--;
                        continue;
                    }

                    NetworkStream stream = clients[i].Connection.GetStream();

                    if (!stream.DataAvailable)
                    {
                        continue;
                    }

                    byte[] recBuffer = new byte[1024];
                    stream.Read(recBuffer, 0, recBuffer.Length);

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new BindChanger();
                    formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    MemoryStream ms = new MemoryStream(recBuffer);
                    NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                    Receive(msg, clients[i]);
                }
            }
        }

        public void Broadcast(NetMsg msg)
        {
            lock (_locker)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    Send(msg, clients[i].Connection);
                }
            }
        }

        public void BroadcastEx(NetMsg msg, short token)
        {
            lock (_locker)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].Token == token) continue;
                    Send(msg, clients[i].Connection);
                }
            }
        }

        public void SendToClient(NetMsg msg, short token)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Token == token)
                {
                    Send(msg, clients[i].Connection);
                }
            }
        }

        private void Send(NetMsg msg, TcpClient connection)
        {
            byte[] buffer = new byte[1024];

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(buffer);
            formatter.Serialize(ms, msg);

            try
            {
                NetworkStream stream = connection.GetStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                Log("Weirdness, Client disconnected..");
            }
        }

        private void Receive(NetMsg msg, ClientConnection client)
        {
            switch (msg.OP)
            {
                case NetOP.RequestToken:
                    {
                        short token = current_token++;
                        client.Token = token;

                        Log("Received token request from client, giving them the token id of " + token);
                        Send(new Net_AssignToken() { Token = token }, client.Connection);
                    }
                    break;
            }
            if (OnNetMsgReceived.ContainsKey(msg.OP))
            {
                OnNetMsgReceived[msg.OP](msg, client.Token);
            }
            else
            {
                BroadcastEx(msg, client.Token);
            }
        }

        public void AddCallback(NetOP op, Action<NetMsg, short> action)
        {
            if (OnNetMsgReceived != null)
            {
                if (OnNetMsgReceived.ContainsKey(op))
                {
                    OnNetMsgReceived[op] += action;
                }
                else
                {
                    OnNetMsgReceived.Add(op, action);
                }
            }
        }

        public void RemoveCallback(NetOP op, Action<NetMsg, short> action)
        {
            if (OnNetMsgReceived != null && OnNetMsgReceived.ContainsKey(op))
            {
                OnNetMsgReceived[op] -= action;
            }
        }

        private void Log(string msg)
        {
            //Ymit.UI.DebugFadeLabelMouse("Server: " + msg);
            Console.WriteLine("Server: " + msg);
        }
    }
}
