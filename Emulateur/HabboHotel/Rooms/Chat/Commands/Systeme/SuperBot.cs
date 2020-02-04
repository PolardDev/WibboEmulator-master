using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.RoomBots;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SuperBot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length > 1)
            {
                int nombre = 1;

                if (Params.Length == 2)
                {
                    int.TryParse(Params[1], out nombre);
                    for (int i = 0; i < nombre; i++)
                    {
                        if (!Room.RpRoom)
                        {
                            RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, Session.GetHabbo().Username, "SuperBot", Session.GetHabbo().Gender, Session.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                            superBot.BotData.FollowUser = UserRoom.VirtualId;
                        } else
                        {
                            RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, Session.GetHabbo().Username, "SuperBot", Session.GetHabbo().Gender, Session.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        }
                    }
                } else if (Params.Length > 2)
                {
                    RoomUser GetUserRoom = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                    if (GetUserRoom == null)
                        return;

                    if (Session.Langue != GetUserRoom.GetClient().Langue)
                    {
                        UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", GetUserRoom.GetClient().Langue), Session.Langue));
                        return;
                    }

                    int.TryParse(Params[2], out nombre);
                    for (int i = 0; i < nombre; i++)
                    {
                        RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, GetUserRoom.GetClient().GetHabbo().Id, Room.Id, AIType.SuperBot, false, GetUserRoom.GetClient().GetHabbo().Username, "SuperBot", GetUserRoom.GetClient().GetHabbo().Gender, GetUserRoom.GetClient().GetHabbo().Look, GetUserRoom.X, GetUserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.BotData.FollowUser = GetUserRoom.VirtualId;
                    }
                }
            }
            else
            {

                int Id = 1;
                for (int Y = 0; Y < Room.GetGameMap().Model.MapSizeY; ++Y)
                {
                    for (int X = 0; X < Room.GetGameMap().Model.MapSizeX; ++X)
                    {
                        if (!Room.GetGameMap().CanWalk(X, Y, false))
                            continue;

                        Id++;

                        RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-Id, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, Session.GetHabbo().Username, Session.GetHabbo().Motto, Session.GetHabbo().Gender, Session.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.MoveTo(X, Y, true);
                    }
                }
            }
        }
    }
}
