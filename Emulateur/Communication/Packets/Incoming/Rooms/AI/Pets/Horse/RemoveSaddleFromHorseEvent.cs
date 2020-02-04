using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;namespace Butterfly.Communication.Packets.Incoming.Structure{    class RemoveSaddleFromHorseEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser PetUser = null;
            if (!Room.GetRoomUserManager().TryGetPet(Packet.PopInt(), out PetUser))
                return;

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id || PetUser.PetData.Type != 13)
                return;

            //Fetch the furniture Id for the pets current saddle.
            int SaddleId = ItemUtility.GetSaddleId(PetUser.PetData.Saddle);

            //Remove the saddle from the pet.
            PetUser.PetData.Saddle = 0;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_pets` SET `have_saddle` = '0' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
            }

            //Give the saddle back to the user.
            ItemData ItemData = null;
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(SaddleId, out ItemData))
                return;

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), "");
            if (Item != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);
                Session.SendPacket(new FurniListNotificationComposer(Item.Id, 1));
                Session.SendPacket(new PurchaseOKComposer());
            }

            //Update the Pet and the Pet figure information.
            Room.SendPacket(new UsersComposer(PetUser));
            Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));        }    }}