namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ModeratorRoomInfoMessageComposer : ServerPacket
    {
        public ModeratorRoomInfoMessageComposer()
            : base(ServerPacketHeader.ModeratorRoomInfoMessageComposer)
        {
			
        }
    }
}
