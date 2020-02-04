namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class FriendListUpdateMessageComposer : ServerPacket
    {
        public FriendListUpdateMessageComposer()
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
			
        }
    }
}
