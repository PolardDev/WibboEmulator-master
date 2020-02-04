namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class AvatarEffectAddedMessageComposer : ServerPacket
    {
        public AvatarEffectAddedMessageComposer()
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
			
        }
    }
}
