using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using System.Collections.Generic;
using Butterfly.Communication.Packets.Outgoing.Structure;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class extrabox : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int NbLot = 0;

            int.TryParse(Params[1], out NbLot);

            if (NbLot < 0 || NbLot > 10)
                return;

            ItemData ItemData = null;
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(12018410, out ItemData))
                return;

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", NbLot);
            foreach (Item PurchasedItem in Items)
            {
                if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    Session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }
        }
    }
}
