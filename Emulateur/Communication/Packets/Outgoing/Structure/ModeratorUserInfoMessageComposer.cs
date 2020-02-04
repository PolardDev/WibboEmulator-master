namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ModeratorUserInfoMessageComposer : ServerPacket
    {
        public ModeratorUserInfoMessageComposer()
            : base(ServerPacketHeader.ModeratorUserInfoMessageComposer)
        {
			
        }
    }
}
