namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class FlatAccessDeniedMessageComposer : ServerPacket
    {
        public FlatAccessDeniedMessageComposer()
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
			
        }
    }
}
