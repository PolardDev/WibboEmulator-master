using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class DanceComposer : ServerPacket
    {
        public DanceComposer(RoomUser Avatar, int Dance)
            : base(ServerPacketHeader.DanceMessageComposer)
        {
            WriteInteger(Avatar.VirtualId);
            WriteInteger(Dance);
        }
    }
}
