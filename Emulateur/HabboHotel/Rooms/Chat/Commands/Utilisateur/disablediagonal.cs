using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class disablediagonal : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            currentRoom.GetGameMap().DiagonalEnabled = !currentRoom.GetGameMap().DiagonalEnabled;

        }
    }
}
