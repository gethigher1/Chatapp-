using System;
using System.IO;
using System.Text;

namespace Chat_Szerver.Net.IO
{
    class PacketBuilder  //data -> memory stream 
    {
        MemoryStream ms;
        public PacketBuilder()
        {
            ms = new MemoryStream(); //3 funkcio write/
        }

        public void WriteOpCode(byte opcode)
        {
            ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            var msgLength = msg.Length; //payload
            ms.Write(BitConverter.GetBytes(msgLength));
            ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return ms.ToArray();
        }

    }
}
