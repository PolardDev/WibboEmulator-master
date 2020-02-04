using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class moonwalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.team != Team.none || UserRoom.InGame)
                return;

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null || UserRoom.InGame)
                return;
            RoomUser roomUserByHabbo = UserRoom;
            roomUserByHabbo.moonwalkEnabled = !roomUserByHabbo.moonwalkEnabled;
            if (roomUserByHabbo.moonwalkEnabled)
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.false", Session.Langue));

        }
    }
}
