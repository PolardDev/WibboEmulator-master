using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Users;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(Habbo habbo, GameClient session, List<Group> groups, int friendCount)
            : base(ServerPacketHeader.ProfileInformationMessageComposer)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(habbo.AccountCreated);

            WriteInteger(habbo.Id);
            WriteString(habbo.Username);
            WriteString(habbo.Look);
            WriteString(habbo.Motto);
            WriteString(origin.ToString("dd/MM/yyyy"));
            WriteInteger(habbo.AchievementPoints);
            WriteInteger(friendCount); // Friend Count
            WriteBoolean(habbo.Id != session.GetHabbo().Id && session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id)); //  Is friend
            WriteBoolean(habbo.Id != session.GetHabbo().Id && !session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id) && session.GetHabbo().GetMessenger().RequestExists(habbo.Id)); // Sent friend request
            WriteBoolean((ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(habbo.Id)) != null);

            WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                WriteInteger(group.Id);
                WriteString(group.Name);
                WriteString(group.Badge);
                WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                WriteBoolean(habbo.FavouriteGroupId == group.Id); // todo favs
                WriteInteger(0);//what the fuck
                WriteBoolean(group != null ? group.ForumEnabled : true);//HabboTalk
            }

            WriteInteger(Convert.ToInt32(ButterflyEnvironment.GetUnixTimestamp() - habbo.LastOnline)); // Last online
            WriteBoolean(true); // Show the profile
        }
    }
}
