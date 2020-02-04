namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class FigureSetIdsComposer : ServerPacket
    {
        public FigureSetIdsComposer()
            : base(ServerPacketHeader.FigureSetIdsMessageComposer)
        {
            WriteInteger(0);

            WriteInteger(0);
        }
    }
}
