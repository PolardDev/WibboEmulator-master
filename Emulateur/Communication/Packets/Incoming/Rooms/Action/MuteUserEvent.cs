using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class MuteUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || (room.RoomData.MuteFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
                return;
            int pId = Packet.PopInt();
            int num = Packet.PopInt();
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(pId);
            int Time = Packet.PopInt() * 60;
            if (roomUserByHabbo == null || roomUserByHabbo.IsBot || (room.CheckRights(roomUserByHabbo.GetClient(), true) || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_mod") || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_no_mute")))
                return;
            room.AddMute(pId, Time);

        }
    }
}
