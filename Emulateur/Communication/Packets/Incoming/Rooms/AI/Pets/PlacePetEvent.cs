using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Pets;using Butterfly.HabboHotel.RoomBots;using Butterfly.HabboHotel.Rooms;using System;

namespace Butterfly.Communication.Packets.Incoming.Structure{    class PlacePetEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            Room Room = null;
            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(1));
                return;
            }            if (Room.GetRoomUserManager().BotCount >= 20)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.placepet.error", Session.Langue));                return;            }            Pet Pet = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryGetPet(Packet.PopInt(), out Pet))
                return;

            if (Pet == null)
                return;

            if (Pet.PlacedInRoom)
                return;            int X = Packet.PopInt();            int Y = Packet.PopInt();            if (!Room.GetGameMap().CanWalk(X, Y, false))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(4));
                return;
            }            RoomUser OldPet = null;
            if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out OldPet))
            {
                Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
            }            Pet.X = X;
            Pet.Y = Y;

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.Id;            Room.GetRoomUserManager().DeployBot(new RoomBot(Pet.PetId, Pet.OwnerId, Pet.RoomId, AIType.Pet, true, Pet.Name, "", "", Pet.Look, X, Y, 0, 0, false, "", 0, false, 0, 0, 0), Pet);            if (Pet.DBState != DatabaseUpdateState.NeedsInsert)                Pet.DBState = DatabaseUpdateState.NeedsUpdate;                        using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())            {                queryreactor.RunQuery("UPDATE user_pets SET room_id = '" + Pet.RoomId + "' WHERE id ='" + Pet.PetId + "' LIMIT 1");
            }            Pet ToRemove = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryRemovePet(Pet.PetId, out ToRemove))
            {
                Console.WriteLine("Error whilst removing pet: " + ToRemove.PetId);
                return;
            }            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));        }    }}