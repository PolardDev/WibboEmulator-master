namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class FurniListUpdateComposer : ServerPacket
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.FurniListUpdateMessageComposer)
        {
			
        }
    }
}
