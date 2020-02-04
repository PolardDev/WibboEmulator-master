namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class UserNameChangeMessageComposer : ServerPacket
    {
        public UserNameChangeMessageComposer(string Name, int VirtualId)
            : base(ServerPacketHeader.UserNameChangeMessageComposer)
        {
            WriteInteger(0);
            WriteInteger(VirtualId);
            WriteString(Name);
        }
    }
}
