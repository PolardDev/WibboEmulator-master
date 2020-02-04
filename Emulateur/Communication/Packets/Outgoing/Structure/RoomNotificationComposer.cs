namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class RoomNotificationComposer : ServerPacket
    {
        public RoomNotificationComposer(string Type, string Key, string Value)
           : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            WriteString(Type);
            WriteInteger((Type == "furni_placement_error") ? 2 : 1);//Count
            {
                if(Type == "furni_placement_error")
                {
                    WriteString("display");
                    WriteString("BUBBLE");
                }
                WriteString(Key);//Type of message
                WriteString(Value);
            }
        }

        public RoomNotificationComposer(string Type)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            WriteString(Type);
            WriteInteger(0);//Count
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string HotelName, string HotelURL)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            int CountMessage = 2;
            if (!string.IsNullOrEmpty(HotelName))
                CountMessage = CountMessage + 2;

            WriteString(Image);
            WriteInteger(CountMessage);
            WriteString("title");
            WriteString(Title);
            WriteString("message");
            WriteString(Message);

            if (!string.IsNullOrEmpty(HotelName))
            {
                WriteString("linkUrl");
                WriteString(HotelURL);
                WriteString("linkTitle");
                WriteString(HotelName);
            }
        }
    }
}
