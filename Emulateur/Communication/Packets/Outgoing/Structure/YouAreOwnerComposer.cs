namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class YouAreOwnerComposer : ServerPacket
    {
        public YouAreOwnerComposer()
            : base(ServerPacketHeader.YouAreOwnerMessageComposer)
        {
			
        }
    }
}
