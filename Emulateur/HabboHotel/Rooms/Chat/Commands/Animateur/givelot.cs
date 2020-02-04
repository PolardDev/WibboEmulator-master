using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Items;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    class givelot : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)                return;            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)                return;            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));                return;            }            if (roomUserByHabbo.GetUsername() == Session.GetHabbo().Username || roomUserByHabbo.GetClient().GetHabbo().IP == Session.GetHabbo().IP)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", Session.Langue));                ButterflyEnvironment.GetGame().GetModerationTool().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + roomUserByHabbo.GetUsername());                return;            }            int NbLot = ButterflyEnvironment.GetRandomNumber(1, 3);            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)                NbLot = ButterflyEnvironment.GetRandomNumber(3, 5);                        ItemData ItemData = null;
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(12018410, out ItemData))
                return;
            
            int NbBadge = ButterflyEnvironment.GetRandomNumber(1, 2);            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)                NbBadge = ButterflyEnvironment.GetRandomNumber(2, 3);

            ItemData ItemDataBadge = null;
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(91947063, out ItemDataBadge))
                return;

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, roomUserByHabbo.GetClient().GetHabbo(), "", NbLot);            Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, roomUserByHabbo.GetClient().GetHabbo(), "", NbBadge));            foreach (Item PurchasedItem in Items)
            {
                if (roomUserByHabbo.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    roomUserByHabbo.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }            roomUserByHabbo.GetClient().SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByHabbo.GetClient().Langue), NbLot, NbBadge));            UserRoom.SendWhisperChat(roomUserByHabbo.GetUsername() + " à reçu " + NbLot + " ExtraBox et " + NbBadge + " BadgeBox!");            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())                queryreactor.RunQuery("UPDATE users SET game_points = game_points + 1, game_points_month = game_points_month + 1 WHERE id = '" + roomUserByHabbo.GetClient().GetHabbo().Id + "';");        }    }}