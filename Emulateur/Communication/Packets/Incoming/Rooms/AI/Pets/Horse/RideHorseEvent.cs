using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System;
using System.Drawing;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RideHorseEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
                return;

            int PetId = Packet.PopInt();
            bool Type = Packet.PopBoolean();

            RoomUser Pet = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out Pet))
                return;

            if (Pet.PetData == null || Pet.PetData.Type != 13)
                return;

            if (!Pet.PetData.AnyoneCanRide && Pet.PetData.OwnerId != User.UserId)
            {
                return;
            }

            if (Math.Abs(User.X - Pet.X) >= 2 || Math.Abs(User.Y - Pet.Y) >= 2)
            {
                User.MoveTo(Pet.X, Pet.Y);
                return;
            }

            if (Type && !User.RidingHorse)
            {
                if (Pet.RidingHorse)
                {
                    string Speechtxt = ButterflyEnvironment.GetLanguageManager().TryGetValue("pet.alreadymounted", Session.Langue);
                    Pet.OnChat(Speechtxt, 0, false);
                }
                else if (User.RidingHorse)
                {
                    return;
                }
                else
                {
                    if (Pet._statusses.Count > 0)
                        Pet._statusses.Clear();

                    int NewX2 = Pet.X;
                    int NewY2 = Pet.Y;
                    Room.SendPacket(Room.GetRoomItemHandler().UpdateUserOnRoller(User, new Point(NewX2, NewY2), 0, Room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2) + 1));
                    Room.SendPacket(Room.GetRoomItemHandler().UpdateUserOnRoller(Pet, new Point(NewX2, NewY2), 0, Room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2)));

                    User.MoveTo(NewX2, NewY2);

                    User.RidingHorse = true;
                    Pet.RidingHorse = true;
                    Pet.HorseID = User.VirtualId;
                    User.HorseID = Pet.VirtualId;

                    if (Pet.PetData.Saddle == 9)
                        User.ApplyEffect(77);
                    else
                        User.ApplyEffect(103);

                    User.RotBody = Pet.RotBody;
                    User.RotHead = Pet.RotHead;

                    User.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
            }
            else
            {
                if (User.VirtualId == Pet.HorseID)
                {
                    Pet._statusses.Remove("sit");
                    Pet._statusses.Remove("lay");
                    Pet._statusses.Remove("snf");
                    Pet._statusses.Remove("eat");
                    Pet._statusses.Remove("ded");
                    Pet._statusses.Remove("jmp");
                    User.RidingHorse = false;
                    User.HorseID = 0;
                    Pet.RidingHorse = false;
                    Pet.HorseID = 0;
                    User.MoveTo(new Point(User.X + 1, User.Y + 1));
                    User.ApplyEffect(-1);
                    User.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
            }

            Room.SendPacket(new PetHorseFigureInformationComposer(Pet));
        }
    }
}
