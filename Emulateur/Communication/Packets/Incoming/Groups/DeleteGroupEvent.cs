﻿using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Group Group = null;
            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().HasFuse("group_delete_override"))//Maybe a FUSE check for staff override?
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.1", Session.Langue));
                return;
            }

            if (Group.MemberCount >= 500 && !Session.GetHabbo().HasFuse("group_delete_limit_override"))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.2", Session.Langue));
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.RoomData.Group = null;
            }

            //Remove it from the cache.
            ButterflyEnvironment.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            //Now the :S stuff.
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("UPDATE `rooms` SET `groupId` = '0' WHERE `groupId` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Group.Id + "' LIMIT 1");
            }

            //Unload it last.
            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

            //Say hey!
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", Session.Langue));
            return;
        }
    }
}
