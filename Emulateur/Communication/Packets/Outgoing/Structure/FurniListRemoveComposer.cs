namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class FurniListRemoveComposer : ServerPacket
    {
        public FurniListRemoveComposer(int Id)
            : base(ServerPacketHeader.FurniListRemoveMessageComposer)
        {
            WriteInteger(Id);
        }
    }
}
