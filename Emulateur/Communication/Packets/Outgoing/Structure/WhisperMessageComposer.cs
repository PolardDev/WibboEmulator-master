namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class WhisperMessageComposer : ServerPacket
    {
        public WhisperMessageComposer()
            : base(ServerPacketHeader.WhisperMessageComposer)
        {
			
        }
    }
}
