using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class handitem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            if (UserRoom.team != Team.none || UserRoom.InGame)
                return;

            int handitemid = -1;
            int.TryParse(Params[1], out handitemid);
            if (handitemid < 0)
                return;

            UserRoom.CarryItem(handitemid);

        }
    }
}
