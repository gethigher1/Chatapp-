using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Chat_Szerver.Net.IO
{
    class PacketReader: BinaryReader //refrektálja amit írunk
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer; //temporary buffer
            var length = ReadInt32();
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length);

            var msg =  Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }

    }
}