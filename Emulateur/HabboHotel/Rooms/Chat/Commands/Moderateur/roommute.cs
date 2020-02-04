using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class roommute : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Session.GetHabbo().CurrentRoom.RoomMuted = !Session.GetHabbo().CurrentRoom.RoomMuted;

        }
    }
}
