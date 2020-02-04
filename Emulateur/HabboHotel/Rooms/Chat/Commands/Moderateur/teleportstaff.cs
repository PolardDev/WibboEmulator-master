using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class teleportstaff : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            RoomUser roomUserByHabbo = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            roomUserByHabbo.TeleportEnabled = !roomUserByHabbo.TeleportEnabled;
        }
    }
}
