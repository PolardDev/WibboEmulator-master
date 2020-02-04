using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    class enable : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)                return;            int NumEnable;            if (!int.TryParse(Params[1], out NumEnable))                return;            if (!ButterflyEnvironment.GetGame().GetEffectsInventoryManager().EffectExist(NumEnable, Session.GetHabbo().HasFuse("fuse_mod")))
                return;            if (UserRoom.team != Team.none || UserRoom.InGame)
                return;            int CurrentEnable = UserRoom.CurrentEffect;            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 184)
                return;

            UserRoom.ApplyEffect(NumEnable);        }    }}