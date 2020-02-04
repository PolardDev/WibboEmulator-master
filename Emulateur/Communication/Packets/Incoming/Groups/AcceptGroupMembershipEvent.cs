using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Users;
using Butterfly.Communication.Packets.Outgoing.Structure;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class AcceptGroupMembershipEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if ((Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id)) && !Session.GetHabbo().HasFuse("fuse_group_accept_any"))
                return;

            if (!Group.HasRequest(UserId))
                return;

            Habbo Habbo = ButterflyEnvironment.GetHabboById(UserId);
            if (Habbo == null)
                return;

            Group.HandleRequest(UserId, true);

            Habbo.MyGroups.Add(Group.Id);

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, Habbo, 4));
        }
    }
}