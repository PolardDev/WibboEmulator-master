using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class freeze : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;
            RoomUser roomUserByHabbo = UserRoom.mRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
            if (roomUserByHabbo == null)
                return;

            roomUserByHabbo.Freeze = !roomUserByHabbo.Freeze;
            roomUserByHabbo.FreezeEndCounter = 0;


        }
    }
}
