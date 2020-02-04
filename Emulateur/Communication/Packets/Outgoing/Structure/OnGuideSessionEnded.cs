namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class OnGuideSessionEnded : ServerPacket
    {
        public OnGuideSessionEnded()
            : base(ServerPacketHeader.OnGuideSessionEnded)
        {
			
        }
    }
}
