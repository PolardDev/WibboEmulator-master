namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class OnGuideSessionDetached : ServerPacket
    {
        public OnGuideSessionDetached()
            : base(ServerPacketHeader.OnGuideSessionDetached)
        {
			
        }
    }
}
