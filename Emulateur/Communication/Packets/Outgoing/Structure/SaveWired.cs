namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class SaveWired : ServerPacket
    {
        public SaveWired()
            : base(ServerPacketHeader.WiredSavedComposer)
        {
			
        }
    }
}
