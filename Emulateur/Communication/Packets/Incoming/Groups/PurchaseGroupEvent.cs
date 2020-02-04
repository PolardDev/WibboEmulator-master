using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class PurchaseGroupEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string Name = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            int RoomId = packet.PopInt();
            int Colour1 = packet.PopInt();
            int Colour2 = packet.PopInt();
            int Unknown = packet.PopInt();

            int groupCost = 20;

            if (session.GetHabbo().Credits < groupCost)
                return;

            session.GetHabbo().Credits -= groupCost;
            session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));

            RoomData Room = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Room == null || Room.OwnerId != session.GetHabbo().Id || Room.Group != null)
                return;

            string Badge = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
            }

            Group Group = null;
            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryCreateGroup(session.GetHabbo(), Name, Description, RoomId, Badge, Colour1, Colour2, out Group))
                return;

            session.SendPacket(new PurchaseOKComposer());

            Room.Group = Group;

            if (session.GetHabbo().CurrentRoomId != Room.Id)
                session.SendPacket(new RoomForwardComposer(Room.Id));

            session.SendPacket(new NewGroupInfoComposer(RoomId, Group.Id));
        }
    }
}