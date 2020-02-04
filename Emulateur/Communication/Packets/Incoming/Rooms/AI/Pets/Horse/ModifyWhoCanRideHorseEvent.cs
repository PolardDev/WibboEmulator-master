using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms;namespace Butterfly.Communication.Packets.Incoming.Structure{    class ModifyWhoCanRideHorseEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int PetId = Packet.PopInt();

            RoomUser Pet = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out Pet))
                return;

            if (Pet.PetData == null || Pet.PetData.Type != 13)
                return;

            if (Pet.PetData.AnyoneCanRide)
                Pet.PetData.AnyoneCanRide = false;
            else
                Pet.PetData.AnyoneCanRide = true;

            if(!Pet.PetData.AnyoneCanRide)
            {
                if (Pet.RidingHorse)
                {
                    Pet.RidingHorse = false;
                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
                    if (User != null)
                    {
                        if (Room.CheckRights(User.GetClient(), true))
                        {
                            User.RidingHorse = false;
                            User.HorseID = 0;
                            User.ApplyEffect(-1);
                            User.MoveTo(User.X + 1, User.Y + 1);
                        }
                    }
                }
            }


            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_pets` SET `anyone_ride` = '" + ButterflyEnvironment.BoolToEnum(Pet.PetData.AnyoneCanRide) + "' WHERE `id` = '" + PetId + "' LIMIT 1");
            }

            Room.SendPacket(new PetInformationComposer(Pet.PetData, Pet.RidingHorse));        }    }}