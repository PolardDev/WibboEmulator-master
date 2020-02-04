using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class LoadRoomItems : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            int RoomId;            if (!int.TryParse(Params[1], out RoomId))                return;
            
            Room.GetRoomItemHandler().LoadFurniture(RoomId);
            Room.GetGameMap().GenerateMaps();
            UserRoom.SendWhisperChat("Mobi de l'appart " + RoomId + " chargé!");
        }
    }
}
