namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class CheckPetNameMessageComposer : ServerPacket
    {
        public CheckPetNameMessageComposer()
            : base(ServerPacketHeader.CheckPetNameMessageComposer)
        {
			
        }
    }
}
