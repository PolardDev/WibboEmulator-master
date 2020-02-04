using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class SitEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
			Room room = Session.GetHabbo().CurrentRoom;            if (room == null)                return;            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            if (roomUserByHabbo == null)                return;            if (roomUserByHabbo._statusses.ContainsKey("sit") || roomUserByHabbo._statusses.ContainsKey("lay"))                return;            if (roomUserByHabbo.RotBody % 2 == 0)            {                roomUserByHabbo.SetStatus("sit", "0.5");                roomUserByHabbo.IsSit = true;                roomUserByHabbo.UpdateNeeded = true;            }
        }
    }
}
