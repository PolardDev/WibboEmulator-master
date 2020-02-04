using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class PickupObjectEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Packet.PopInt();
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;
            Item Item = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null)
                return;

            if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            Session.GetHabbo().GetInventoryComponent().AddItem(Item);
            //Session.GetHabbo().GetInventoryComponent().UpdateItems();
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PICK, 0);

        }
    }
}
