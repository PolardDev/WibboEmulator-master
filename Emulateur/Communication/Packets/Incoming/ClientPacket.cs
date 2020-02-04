using System;
using System.Text;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Incoming
{
    public class ClientPacket
    {
        private byte[] Body;
        private int Pointer;
        private readonly Encoding Encoding = Encoding.GetEncoding("Windows-1252");

        public ClientPacket(int messageID, byte[] body)
        {
            Init(messageID, body);
        }

        public int Id { get; private set; }

        public int RemainingLength
        {
            get { return Body.Length - Pointer; }
        }

        public int Header
        {
            get { return Id; }
        }

        public void Init(int messageID, byte[] body)
        {
            if (body == null)
                body = new byte[0];

            Id = messageID;
            Body = body;

            Pointer = 0;
        }

        public byte[] ReadBytes(int Bytes)
        {
            if (Bytes > RemainingLength || Bytes < 0)
                Bytes = RemainingLength;

            byte[] data = new byte[Bytes];
            for (int i = 0; i < Bytes; i++)
                data[i] = Body[Pointer++];

            return data;
        }

        public byte[] ReadFixedValue()
        {
            int len = 0;
            if(RemainingLength >= 2)
                len = HabboEncoding.DecodeInt16(ReadBytes(2));
            return ReadBytes(len);
        }

        public string PopString()
        {
            return Encoding.GetString(ReadFixedValue());
        }

        public bool PopBoolean()
        {
            if (RemainingLength > 0 && Body[Pointer++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        public int PopInt()
        {
            if (RemainingLength < 4)
            {
                return 0;
            }

            byte[] Data = ReadBytes(4);

            Int32 i = HabboEncoding.DecodeInt32(Data);

            return i;
        }

        public override string ToString()
        {
            return "[" + Header + "] BODY: " + (Encoding.GetString(Body).Replace(Convert.ToChar(0).ToString(), "[0]"));
        }

    }
}
