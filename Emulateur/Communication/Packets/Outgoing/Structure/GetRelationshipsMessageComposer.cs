namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class GetRelationshipsMessageComposer : ServerPacket
    {
        public GetRelationshipsMessageComposer()
            : base(ServerPacketHeader.GetRelationshipsMessageComposer)
        {
			
        }
    }
}
