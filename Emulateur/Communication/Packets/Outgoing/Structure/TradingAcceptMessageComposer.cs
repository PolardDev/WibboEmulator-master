namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class TradingAcceptMessageComposer : ServerPacket
    {
        public TradingAcceptMessageComposer()
            : base(ServerPacketHeader.TradingAcceptMessageComposer)
        {
			
        }
    }
}
