namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class SetGroupIdMessageComposer : ServerPacket
    {
        public SetGroupIdMessageComposer()
            : base(ServerPacketHeader.SetGroupIdMessageComposer)
        {
			
        }
    }
}
