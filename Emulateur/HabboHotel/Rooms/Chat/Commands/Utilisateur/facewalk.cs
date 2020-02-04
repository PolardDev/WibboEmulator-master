using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class facewalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.team != Team.none || UserRoom.InGame)
                return;

            RoomUser roomUserByHabbo = UserRoom;
            roomUserByHabbo.facewalkEnabled = !roomUserByHabbo.facewalkEnabled;
            if (roomUserByHabbo.facewalkEnabled)
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.false", Session.Langue));
        }
    }
}
