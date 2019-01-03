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
    public class SocketClient : INetClient
    {
        private short Token;
        private TcpClient server;

        public void ConnectToServer()
        {
            server = new TcpClient("127.0.0.1", 8090);
            Ymit.UI.DebugFadeLabelMouse("Connected to server");
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
                Ymit.UI.DebugFadeLabelMouse("Client: Received token: " + nat.Token);
            }
            else
            {
                Ymit.UI.DebugFadeLabelMouse("Client: Didn't understand message type.");
            }
        }

        public void Process() {
            byte[] recBuffer = new byte[1024];
            NetworkStream stream = server.GetStream();
            if (!stream.DataAvailable)
            {
                return;
            }

            stream.Read(recBuffer, 0, recBuffer.Length);

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(recBuffer);
            NetMsg msg = (NetMsg)formatter.Deserialize(ms);

            Receive(msg);
        }
    }
}
