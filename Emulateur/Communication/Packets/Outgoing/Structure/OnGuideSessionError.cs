namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class OnGuideSessionError : ServerPacket
    {
        public OnGuideSessionError()
            : base(ServerPacketHeader.OnGuideSessionError)
        {
			
        }
    }
}
