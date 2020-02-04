using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    class RpBotChooseEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            string Message = Packet.PopString();

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
                return;

            if (Room.AllowsShous(User, Message))
                User.SendWhisperChat(Message, false);
        }
    }
}