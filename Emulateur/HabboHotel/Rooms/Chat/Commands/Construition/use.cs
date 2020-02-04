using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class use : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            string Count = Params[1];
            int UseCount;
            if (!int.TryParse(Count, out UseCount))
                return;
            if (UseCount < 0 || UseCount > 100)
                return;

            UserRoom.UseMode = true;
            UserRoom.UseCount = UseCount;
        }
    }
}
