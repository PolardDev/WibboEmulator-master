using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class FollowFriendEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Packet.PopInt());
            if (clientByUserId == null || clientByUserId.GetHabbo() == null || !clientByUserId.GetHabbo().InRoom || (clientByUserId.GetHabbo().HideInRoom && !Session.GetHabbo().HasFuse("fuse_mod")))
                return;

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            ServerPacket Response = new ServerPacket(ServerPacketHeader.RoomForwardMessageComposer);
            Response.WriteInteger(clientByUserId.GetHabbo().CurrentRoomId);
            Session.SendPacket(Response);
        }
    }
}