using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class youtube : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
                return;

            string username = Params[1];
            string Videoid = Params[2];

            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(username);
            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null || roomUserByHabbo.GetClient().GetHabbo() == null)
                return;

            if (Session.Langue != roomUserByHabbo.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByHabbo.GetClient().Langue), Session.Langue));
                return;
            }

            roomUserByHabbo.GetClient().GetHabbo().SendWebPacket(new YoutubeTvComposer(0, Videoid));
        }
    }
}
