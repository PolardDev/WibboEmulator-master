using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Users;
using Butterfly.Utilities;
using System;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string Data = Packet.PopString();
            string GiftUser = StringCharFilter.Escape(Packet.PopString());
            string GiftMessage = StringCharFilter.Escape(Packet.PopString().Replace(Convert.ToChar(5), ' '));
            int SpriteId = Packet.PopInt();
            int Ribbon = Packet.PopInt();
            int Colour = Packet.PopInt();
            bool dnow = Packet.PopBoolean();

            CatalogPage Page = null;
            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || Page.MinimumRank > Session.GetHabbo().Rank)
                return;

            CatalogItem Item = null;
            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                return;
            }

            if (!ItemUtility.CanGiftItem(Item))
                return;

            ItemData PresentData = null;
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetGift(SpriteId, out PresentData) || PresentData.InteractionType != InteractionType.GIFT)
                return;

            if (Session.GetHabbo().Credits < Item.CostCredits)
            {
                //Session.SendPacket(new PresentDeliverErrorMessageComposer(true, false));
                return;
            }

            if (Session.GetHabbo().Duckets < Item.CostDuckets)
            {
                //Session.SendPacket(new PresentDeliverErrorMessageComposer(false, true));
                return;
            }

            Habbo Habbo = ButterflyEnvironment.GetHabboByUsername(GiftUser);
            if (Habbo == null)
            {
                //Session.SendPacket(new GiftWrappingErrorComposer());
                return;
            }

            if ((DateTime.Now - Session.GetHabbo().LastGiftPurchaseTime).TotalSeconds <= 15.0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buygift.flood", Session.Langue));

                Session.GetHabbo().GiftPurchasingWarnings += 1;
                if (Session.GetHabbo().GiftPurchasingWarnings >= 25)
                    Session.GetHabbo().SessionGiftBlocked = true;
                return;
            }

            if (Session.GetHabbo().SessionGiftBlocked)
                return;

            string ED = Session.GetHabbo().Id + ";" + GiftMessage +  Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                //Insert the dummy item.
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (@baseId, @habboId, @extra_data)");
                dbClient.AddParameter("baseId", PresentData.Id);
                dbClient.AddParameter("habboId", Habbo.Id);
                dbClient.AddParameter("extra_data", ED);
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                string ItemExtraData = null;
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.none:
                        ItemExtraData = "";
                        break;

                    case InteractionType.GUILD_ITEM:
                    case InteractionType.GUILD_GATE:
                        int Groupid = 0;
                        if (!int.TryParse(Data, out Groupid))
                            return;
                        if (Groupid == 0)
                            return;
                        Group groupItem;
                        if(ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Groupid, out groupItem))
                            ItemExtraData = "0;" + groupItem.Id;
                        break;

                    #region Pet handling

                    case InteractionType.pet:

                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); // to trigger any possible errors

                            if (PetUtility.CheckPetName(PetName))
                                return;

                            if (Race.Length > 2)
                                return;

                            if (Color.Length != 6)
                                return;

                            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        }
                        catch
                        {
                            return;
                        }

                        break;

                    #endregion

                    case InteractionType.FLOOR:
                    case InteractionType.WALLPAPER:
                    case InteractionType.LANDSCAPE:

                        Double Number = 0;
                        try
                        {
                            if (string.IsNullOrEmpty(Data))
                                Number = 0;
                            else
                                Number = Double.Parse(Data);
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break; // maintain extra data // todo: validate

                    case InteractionType.POSTIT:
                        ItemExtraData = "FFFF33";
                        break;

                    case InteractionType.MOODLIGHT:
                        ItemExtraData = "1,1,1,#000000,255";
                        break;

                    case InteractionType.TROPHY:
                        ItemExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + Data;
                        break;

                    case InteractionType.MANNEQUIN:
                        ItemExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                        break;

                    case InteractionType.BADGE_TROC:
                        {
                            string[] BadgeNotAllowed = { "ADM", "GPHWIB", "wibbo.helpeur", "WIBARC", "CRPOFFI", "ZEERSWS", "PRWRD1", "WBI1", "WBI2", "WBI3", "WBI4", "WBI5", "WBI6", "WBI7", "CASINOB", "WPREMIUM", "VIPFREE" };
                            if (BadgeNotAllowed.Any(x => x == Data))
                            {
                                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                                return;
                            }

                            if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                            {
                                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                                return;
                            }

                            Session.GetHabbo().GetBadgeComponent().RemoveBadge(Data);
                            Session.SendPacket(Session.GetHabbo().GetBadgeComponent().Serialize());

                            ItemExtraData = Data;
                            break;
                        }

                    case InteractionType.BADGE_DISPLAY:
                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                            return;

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                //Insert the present, forever.
                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES (@itemId, @baseId, @extra_data)");
                dbClient.AddParameter("itemId", NewItemId);
                dbClient.AddParameter("baseId", Item.Data.Id);
                dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
                dbClient.RunQuery();

                //Here we're clearing up a record, this is dumb, but okay.
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @deleteId LIMIT 1");
                dbClient.AddParameter("deleteId", NewItemId);
                dbClient.RunQuery();
            }


            Item GiveItem = ItemFactory.CreateSingleItem(PresentData, Habbo, ED, NewItemId);
            if (GiveItem != null)
            {
                GameClient Receiver = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Habbo.Id);
                if (Receiver != null)
                {
                    Receiver.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Receiver.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Receiver.SendPacket(new PurchaseOKComposer());
                    //Receiver.SendPacket(new FurniListUpdateComposer());
                }

                if (Habbo.Id != Session.GetHabbo().Id && !string.IsNullOrWhiteSpace(GiftMessage))
                {
                    ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                    if (Receiver != null)
                        ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                }
            }

            Session.SendPacket(new PurchaseOKComposer(Item, PresentData));

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= Item.CostCredits;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostDuckets > 0)
            {
                Session.GetHabbo().Duckets -= Item.CostDuckets;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            }

            Session.GetHabbo().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}
