using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class emblem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ADM"))
                UserRoom.CurrentEffect = 540;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("GPHWIB"))
                UserRoom.CurrentEffect = 557;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur"))
                UserRoom.CurrentEffect = 544;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("WIBARC"))
                UserRoom.CurrentEffect = 546;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("CRPOFFI"))
                UserRoom.CurrentEffect = 570;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ZEERSWS"))
                UserRoom.CurrentEffect = 552;

            if (UserRoom.CurrentEffect > 0)
                Room.SendPacket(new AvatarEffectComposer(UserRoom.VirtualId, UserRoom.CurrentEffect));
        }
    }
}
