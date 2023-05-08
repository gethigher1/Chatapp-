using CHATAPP;
using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class server
    {
        TcpClient client;
        public PacketReader PacketReader;//ha van kapcsolat akkor csak

        public event Action connectedEvent;
        public event Action msgRecievedEvent;
        public event Action userDisconnectEvent;
        public server()//konstruktor
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username) //szerver kapcsolodas function
        {
            if(!client.Connected)//error handling
            {
                client.Connect("127.0.0.1", 8080);
                PacketReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
            
        }
        private void ReadPackets() //offload data
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;

                        case 5:
                            msgRecievedEvent?.Invoke();
                            break;

                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine("ah yes...");
                            break;
                    }
                }
            });
        }   
        public void SendMessageToServer(string message) 
        { 
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}