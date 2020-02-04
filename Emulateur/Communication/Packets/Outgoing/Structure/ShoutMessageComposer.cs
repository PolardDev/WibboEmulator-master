namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ShoutMessageComposer : ServerPacket
    {
        public ShoutMessageComposer()
            : base(ServerPacketHeader.ShoutMessageComposer)
        {
			
        }
    }
}
