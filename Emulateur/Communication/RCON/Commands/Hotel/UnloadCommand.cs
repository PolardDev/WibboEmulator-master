using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.RCON.Commands.Hotel
{
    class UnloadCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
                return false;

            int RoomId = 0;

            if (!int.TryParse(parameters[1], out RoomId))
                return false;
            if (RoomId == 0)
                return false;

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null)
                return false;

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            return true;
        }
    }
}
