using Chat_Szerver.Net.IO;
using Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Szerver
{
     class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client) //ctor
        {
            ClientSocket = client;
            UID = Guid.NewGuid();   //uj GUID generálása
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();




            Console.WriteLine($"[{DateTime.Now}]: Új felhasznalo csatlakozott az alábbi néven: {Username}.");

            Task.Run(() => Process());

        }

        //packet processzor
        void Process()
        {
            while (true) 
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]:Message recieved! {msg}");
                            Program.Program.BroadcastMessage($"[{DateTime.Now}] : [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{ UID.ToString()}]: Disconnected!");
                    Program.Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
