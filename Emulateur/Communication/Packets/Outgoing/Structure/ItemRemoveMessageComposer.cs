namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ItemRemoveMessageComposer : ServerPacket
    {
        public ItemRemoveMessageComposer()
            : base(ServerPacketHeader.ItemRemoveMessageComposer)
        {
			
        }
    }
}
