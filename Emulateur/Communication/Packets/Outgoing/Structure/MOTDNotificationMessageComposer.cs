namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class MOTDNotificationMessageComposer : ServerPacket
    {
        public MOTDNotificationMessageComposer()
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
			
        }
    }
}
