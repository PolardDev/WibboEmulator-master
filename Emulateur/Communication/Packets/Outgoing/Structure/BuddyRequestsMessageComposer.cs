namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class BuddyRequestsMessageComposer : ServerPacket
    {
        public BuddyRequestsMessageComposer()
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
			
        }
    }
}
