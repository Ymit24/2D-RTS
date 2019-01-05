using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Net
{
    using NetMessages;
    using System.Reflection;

    public class SocketClient : INetClient
    {
        private short token = -1;
        private TcpClient server;
        public Dictionary<NetOP, Action<NetMsg>> OnNetMsgReceived { get; set; }

        public short GetToken()
        {
            return token;
        }

        public bool HasToken()
        {
            return token != -1;
        }

        public void ConnectToServer()
        {
            OnNetMsgReceived = new Dictionary<NetOP, Action<NetMsg>>();

            server = new TcpClient("127.0.0.1", 8090);
            Log("Connected to server");
        }

        public void Disconnect()
        {
            if (server != null)
            {
                server.Close();
            }
        }

        public void SendToServer(NetMsg msg) {
            byte[] buffer = new byte[1024];

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(buffer);
            formatter.Serialize(ms, msg);

            NetworkStream stream = server.GetStream();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        public void Receive(NetMsg msg) {
            if (msg.OP == NetOP.AssignToken)
            {
                Net_AssignToken nat = (Net_AssignToken)msg;
                this.token = nat.Token;
                Log("Received token: " + nat.Token);
            }
            if (OnNetMsgReceived.ContainsKey(msg.OP))
            {
                OnNetMsgReceived[msg.OP](msg);
            }
        }

        public void Process() {
            byte[] recBuffer = new byte[1024];
            NetworkStream stream = server.GetStream();

            if (server.Client.Poll(1000, SelectMode.SelectRead) &
                        (server.Client.Available == 0))
            {
                Log("server disconnected");
            }

            if (!stream.DataAvailable)
            {
                return;
            }

            stream.Read(recBuffer, 0, recBuffer.Length);

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new BindChanger();
            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            MemoryStream ms = new MemoryStream(recBuffer);
            NetMsg msg = (NetMsg)formatter.Deserialize(ms);

            Receive(msg);
        }

        public void AddCallback(NetOP op, Action<NetMsg> action)
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

        public void RemoveCallback(NetOP op, Action<NetMsg> action)
        {
            if (OnNetMsgReceived != null && OnNetMsgReceived.ContainsKey(op))
            {
                OnNetMsgReceived[op] -= action;
            }
        }

        public void RequestToken()
        {
            SendToServer(new Net_RequestToken());
        }

        private void Log(string msg)
        {
            //Ymit.UI.DebugFadeLabelMouse("Client: " + msg);
            Console.WriteLine("Client: " + msg);
        }
    }
}
