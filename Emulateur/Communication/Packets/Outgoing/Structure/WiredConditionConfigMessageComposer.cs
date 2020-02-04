namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class WiredConditionConfigMessageComposer : ServerPacket
    {
        public WiredConditionConfigMessageComposer()
            : base(ServerPacketHeader.WiredConditionConfigMessageComposer)
        {
			
        }
    }
}
