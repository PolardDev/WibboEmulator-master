using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SpeedWalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.WalkSpeed = !UserRoom.WalkSpeed;

            if (UserRoom.WalkSpeed)
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.false", Session.Langue));
        }
    }
}
