namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class RespectNotificationMessageComposer : ServerPacket
    {
        public RespectNotificationMessageComposer()
            : base(ServerPacketHeader.RespectNotificationMessageComposer)
        {
			
        }
    }
}
