using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;

using System.Collections;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private Dictionary<string, Badge> Badges;
        private readonly int UserId;

        public int Count
        {
            get
            {
                return this.Badges.Count;
            }
        }

        public int EquippedCount
        {
            get
            {
                int num = 0;
                foreach (Badge badge in (IEnumerable)this.Badges.Values)
                {
                    if (badge.Slot > 0)
                        num++;
                }

                if (num > 5)
                    return 5;

                return num;
            }
        }

        public Dictionary<string, Badge> BadgeList
        {
            get
            {
                return this.Badges;
            }
        }

        public BadgeComponent(int userId, List<Badge> data)
        {
            this.Badges = new Dictionary<string, Badge>();
            foreach (Badge badge in data)
            {
                if (!this.Badges.ContainsKey(badge.Code))
                    this.Badges.Add(badge.Code, badge);
            }

            this.UserId = userId;
        }

        public void Destroy()
        {
            Badges.Clear();
        }

        public bool HasBadgeSlot(string Badge)
        {
            if (this.Badges.ContainsKey(Badge))
                return this.Badges[Badge].Slot > 0;
            else
                return false;
        }

        public Badge GetBadge(string Badge)
        {
            if (this.Badges.ContainsKey(Badge))
                return (Badge)this.Badges[Badge];
            else
                return (Badge)null;
        }

        public bool HasBadge(string Badge)
        {
            if (string.IsNullOrEmpty(Badge))
                return true;

            return this.Badges.ContainsKey(Badge);
        }

        public void GiveBadge(string Badge, bool InDatabase)
        {
            this.GiveBadge(Badge, 0, InDatabase);
        }

        public void GiveBadge(string Badge, int Slot, bool InDatabase)
        {
            if (this.HasBadge(Badge))
                return;
            if (InDatabase)
            {
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + this.UserId + ",@badge," + Slot + ")");
                    queryreactor.AddParameter("badge", Badge);
                    queryreactor.RunQuery();
                }
            }
            this.Badges.Add(Badge, new Badge(Badge, Slot));
        }

        public void ResetSlots()
        {
            foreach (Badge badge in (IEnumerable)this.Badges.Values)
                badge.Slot = 0;
        }

        public void RemoveBadge(string Badge)
        {
            if (!this.HasBadge(Badge))
                return;
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + this.UserId + " LIMIT 1");
                queryreactor.AddParameter("badge", Badge);
                queryreactor.RunQuery();
            }
            this.Badges.Remove(this.GetBadge(Badge).Code);
        }

        public ServerPacket Serialize()
        {
            List<Badge> list = new List<Badge>();
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.BadgesMessageComposer);
            serverMessage.WriteInteger(this.Count);
            foreach (Badge badge in (IEnumerable)this.Badges.Values)
            {
                serverMessage.WriteInteger(0);
                serverMessage.WriteString(badge.Code);
                if (badge.Slot > 0)
                    list.Add(badge);
            }
            serverMessage.WriteInteger(list.Count);
            foreach (Badge badge in list)
            {
                serverMessage.WriteInteger(badge.Slot);
                serverMessage.WriteString(badge.Code);
            }
            return serverMessage;
        }
    }
}
