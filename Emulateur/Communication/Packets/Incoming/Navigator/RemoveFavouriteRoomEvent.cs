using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
   if (Session.GetHabbo() == null)
                return;
            int i = Packet.PopInt();
            RoomData roomdata = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(i);
            if (roomdata == null)
                return;
            Session.GetHabbo().FavoriteRooms.Remove(roomdata);
            ServerPacket Response = new ServerPacket(ServerPacketHeader.UpdateFavouriteRoomMessageComposer);
            Response.WriteInteger(i);
            Response.WriteBoolean(false);
            Session.SendPacket(Response);
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery(string.Concat(new object[4] { "DELETE FROM user_favorites WHERE user_id = ", Session.GetHabbo().Id, " AND room_id = ", i }));

        }
    }
}
