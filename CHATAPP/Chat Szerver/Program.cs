using Chat_Szerver;
using Chat_Szerver.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace Program
{
    class Program
    {
        static List<Client> users;
        static TcpListener listener;
        static void Main(string[] args)
        {
            users = new List<Client>(); 
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"),8080);
            listener.Start();//error handling

            while (true)
            {
                var client = new Client(listener.AcceptTcpClient());    //new instance listener -> tcp client
                users.Add(client);
                BroadcastConnection();
            }
            /*Broadcast the connection to everyone on the server*/
             //közvetítés csatlakozás után

            //Console.WriteLine("Csatlakozott a felhasznalo!");

        }

        static void BroadcastConnection()
        {
            foreach (var user in users) 
            {
                foreach(var usr in users)
                {
                    var broadcastPacket = new PacketBuilder(); //olvashato modon
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
        public static void BroadcastMessage(string message)
        {
            foreach (var user in users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault(); 
            users.Remove(disconnectedUser);

            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectedUser.Username}] Disconnected!");
        }


    }
}