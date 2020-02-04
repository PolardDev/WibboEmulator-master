using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;

namespace Butterfly.Communication.Packets.Incoming.Structure
            Room Room = null;
            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(1));
                return;
            }
            if (!Session.GetHabbo().GetInventoryComponent().TryGetPet(Packet.PopInt(), out Pet))
                return;

            if (Pet == null)
                return;

            if (Pet.PlacedInRoom)
                return;
            {
                //Session.SendPacket(new RoomErrorNotifComposer(4));
                return;
            }
            if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out OldPet))
            {
                Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
            }
            Pet.Y = Y;

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.Id;
            }
            if (!Session.GetHabbo().GetInventoryComponent().TryRemovePet(Pet.PetId, out ToRemove))
            {
                Console.WriteLine("Error whilst removing pet: " + ToRemove.PetId);
                return;
            }