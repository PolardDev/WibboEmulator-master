using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class SleepComposer : ServerPacket
    {
        public SleepComposer(RoomUser User, bool IsSleeping)
            : base(ServerPacketHeader.SleepMessageComposer)
        {
            WriteInteger(User.VirtualId);
            WriteBoolean(IsSleeping);
        }
    }
}
