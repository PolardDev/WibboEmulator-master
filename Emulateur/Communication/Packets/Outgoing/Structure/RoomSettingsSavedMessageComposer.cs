namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class RoomSettingsSavedMessageComposer : ServerPacket
    {
        public RoomSettingsSavedMessageComposer()
            : base(ServerPacketHeader.RoomSettingsSavedMessageComposer)
        {
			
        }
    }
}
