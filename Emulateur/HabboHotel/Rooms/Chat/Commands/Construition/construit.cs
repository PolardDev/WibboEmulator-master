using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    class construit : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)                return;            string Heigth = Params[1];            double Result;            if (double.TryParse(Heigth, out Result))            {                if (Result >= 0.01 && Result <= 10)                {                    UserRoom.ConstruitMode = true;                    UserRoom.ConstruitHeigth = Result;                }            }        }    }}