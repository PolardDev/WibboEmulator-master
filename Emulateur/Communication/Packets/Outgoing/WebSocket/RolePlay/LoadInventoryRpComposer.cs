using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roleplay.Player;
using System.Collections.Concurrent;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    class LoadInventoryRpComposer : ServerPacket
    {
        public LoadInventoryRpComposer(ConcurrentDictionary<int, RolePlayInventoryItem> Items)
          : base(9)
        {
            WriteInteger(Items.Count);

            foreach(RolePlayInventoryItem Item in Items.Values)
            {
                RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Item.ItemId);

                WriteInteger(Item.ItemId);
                WriteString(RpItem.Name);
                WriteString(RpItem.Desc);
                WriteInteger(Item.Count);
                WriteInteger((int)RpItem.Category);
                WriteInteger(RpItem.UseType);
            }
        }
    }
}