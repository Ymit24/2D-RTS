using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Game.Net
{
    public class SocketServer : INetServer
    {
        private TcpListener listener;
        public void Init()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8090);
        }

        private List<TcpClient> clients;
        private Thread listeningThread;

        private static object _locker = new object();

        public void Start()
        {
            clients = new List<TcpClient>();
            listener.Start();
            listeningThread = new Thread(() =>
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (_locker)
                    {
                        clients.Add(client);
                    }
                }
            });
            listeningThread.Start();
        }

        public void Stop()
        {
            if (listeningThread != null)
            {
                listeningThread.Abort();
            }
            if (clients != null)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].Close();
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
                    NetworkStream stream = clients[i].GetStream();
                    if (!stream.DataAvailable)
                    {
                        return;
                    }

                    byte[] recBuffer = new byte[1024];
                    stream.Read(recBuffer, 0, recBuffer.Length);

                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream(recBuffer);
                    NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                    Receive(msg);
                }
            }
        }

        public void Broadcast(NetMsg msg)
        {
            lock (_locker)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    byte[] buffer = new byte[1024];

                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream(buffer);
                    formatter.Serialize(ms, msg);

                    NetworkStream stream = clients[i].GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
            }
        }

        public void Receive(NetMsg msg)
        {
            if (msg.OP == NetOP.AssignToken)
            {
                Log("Server: Received token from client " + ((NetMessages.Net_AssignToken)msg).Token);
            }
            else
            {
                Log("Server: Didn't understand message type.");
            }
        }

        private void Log(string msg)
        {
            Ymit.UI.DebugFadeLabelMouse(msg);
        }
    }
}
