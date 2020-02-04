using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class seteffect : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            int.TryParse(Params[1], out int EnableNum);

            if (!ButterflyEnvironment.GetGame().GetEffectsInventoryManager().EffectExist(EnableNum, Session.GetHabbo().HasFuse("fuse_mod")))
                return;

            UserRoom.ApplyEffect(EnableNum);
        }
    }
}
