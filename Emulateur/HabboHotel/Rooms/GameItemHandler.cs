using Butterfly.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Butterfly.HabboHotel.Groups;

namespace Butterfly.HabboHotel.Rooms
{
    public class GameItemHandler
    {
        private Dictionary<int, Item> banzaiTeleports;
        private Dictionary<int, Item> banzaiPyramids;
        private readonly Dictionary<Point, List<Item>> GroupGate;
        private readonly Dictionary<int, Item> banzaiBlobs;
        private Room room;
        private Item ExitTeleport;

        public GameItemHandler(Room room)
        {
            this.room = room;
            this.banzaiPyramids = new Dictionary<int, Item>();
            this.banzaiTeleports = new Dictionary<int, Item>();
            this.GroupGate = new Dictionary<Point, List<Item>>();
            this.banzaiBlobs = new Dictionary<int, Item>();
        }

        public void OnCycle()
        {
            this.CyclePyramids();
        }

        private void CyclePyramids()
        {
            if (this.banzaiPyramids == null)
                return;

            Random random = new Random();
            foreach (Item roomItem in this.banzaiPyramids.Values.ToList())
            {
                if (roomItem.interactionCountHelper == 0 && roomItem.ExtraData == "1")
                {
                    roomItem.interactionCountHelper = 1;
                }
                if (string.IsNullOrEmpty(roomItem.ExtraData))
                    roomItem.ExtraData = "0";
                if (random.Next(0, 30) == 15)
                {
                    if (roomItem.ExtraData == "0")
                    {
                        roomItem.ExtraData = "1";
                        roomItem.UpdateState();
                        this.room.GetGameMap().updateMapForItem(roomItem);
                    }
                    else if (this.room.GetGameMap().CanStackItem(roomItem.GetX, roomItem.GetY))
                    {
                        roomItem.ExtraData = "0";
                        roomItem.UpdateState();
                        this.room.GetGameMap().updateMapForItem(roomItem);
                    }
                }
            }
        }

      
        public void AddPyramid(Item item, int itemID)
        {
            if (this.banzaiPyramids.ContainsKey(itemID))
                this.banzaiPyramids[itemID] = item;
            else
                this.banzaiPyramids.Add(itemID, item);
        }

        public void RemovePyramid(int itemID)
        {
            this.banzaiPyramids.Remove(itemID);
        }

        public void RemoveBlob(int itemID)
        {
            this.banzaiBlobs.Remove(itemID);
        }

        public Item GetExitTeleport()
        {
            return this.ExitTeleport;
        }

        public void AddExitTeleport(Item item)
        {
            this.ExitTeleport = item;
        }

        public void RemoveExitTeleport(Item item)
        {
            Item exitTeleport = this.ExitTeleport;
            if (exitTeleport != null && item.Id == exitTeleport.Id)
                this.ExitTeleport = (Item)null;
        }

        public void AddBlob(Item item, int itemID)
        {
            if (this.banzaiBlobs.ContainsKey(itemID))
                this.banzaiBlobs[itemID] = item;
            else
                this.banzaiBlobs.Add(itemID, item);
        }

        public void OnWalkableBanzaiBlob(RoomUser User, Item Item)
        {
            if (Item.ExtraData == "1")
                return;
            this.room.GetGameManager().AddPointToTeam(User.team, User);
            Item.ExtraData = "1";
            Item.UpdateState();
        }

        public void OnWalkableBanzaiBlo(RoomUser User, Item Item)
        {
            if (Item.ExtraData == "1")
                return;
            this.room.GetGameManager().AddPointToTeam(User.team, 5, User);
            Item.ExtraData = "1";
            Item.UpdateState();
        }

        public void ResetAllBlob()
        {
            foreach (Item Blob in this.banzaiBlobs.Values)
            {
                if (Blob.ExtraData == "0")
                    continue;
                Blob.ExtraData = "0";
                Blob.UpdateState();
            }
        }

        public void AddGroupGate(Item item)
        {
            if (this.GroupGate.ContainsKey(item.Coordinate))
                ((List<Item>)this.GroupGate[item.Coordinate]).Add(item);
            else
                this.GroupGate.Add(item.Coordinate, new List<Item>() { item });
        }

        public void RemoveGroupGate(Item item)
        {
            if (!this.GroupGate.ContainsKey(item.Coordinate))
                return;
            ((List<Item>)this.GroupGate[item.Coordinate]).Remove(item);
            if (this.GroupGate.Count == 0)
                this.GroupGate.Remove(item.Coordinate);
        }

        public void AddTeleport(Item item, int itemID)
        {
            if (this.banzaiTeleports.ContainsKey(itemID))
            {
                //this.banzaiTeleports.Inner[itemID] = item;
                this.banzaiTeleports.Remove(itemID);
                this.banzaiTeleports.Add(itemID, item);
            }
            else
                this.banzaiTeleports.Add(itemID, item);
        }

        public void RemoveTeleport(int itemID)
        {
            this.banzaiTeleports.Remove(itemID);
        }

        public bool CheckGroupGate(RoomUser User, Point Coordinate)
        {
            if (this.GroupGate == null)
                return false;
            if (!this.GroupGate.ContainsKey(Coordinate))
                return false;
            if (((List<Item>)this.GroupGate[Coordinate]).Count == 0)
                return false;

            Item item = Enumerable.FirstOrDefault<Item>((IEnumerable<Item>)this.GroupGate[Coordinate]);

            Group Group;
            if(!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(item.GroupId, out Group))
                return true;
            if (User == null)
                return false;
            if (User.IsBot)
                return false;
            if (User.GetClient() == null)
                return false;
            if (User.GetClient().GetHabbo() == null)
                return false;
            if (User.GetClient().GetHabbo().Rank > 5)
                return false;
            if (User.GetClient().GetHabbo().MyGroups == null)
                return true;
            if (User.GetClient().GetHabbo().MyGroups.Contains(Group.Id))
                return false;

            return true; //Ne peut pas passer
        }

        public void onTeleportRoomUserEnter(RoomUser User, Item Item)
        {
            IEnumerable<Item> banzaiTeleports2 = banzaiTeleports.Values.Where(p => p.Id != Item.Id);

            int count = banzaiTeleports2.Count();

            if (count == 0)
                return;

            int countID = ButterflyEnvironment.GetRandomNumber(0, count - 1);
            Item BanzaiItem2 = banzaiTeleports2.ElementAt(countID);

            if (BanzaiItem2 == null)
                return;
            if (BanzaiItem2.InteractingUser != 0)
                return;
            User.IsWalking = false;
            User.CanWalk = false;
            BanzaiItem2.InteractingUser = User.UserId;
            BanzaiItem2.ReqUpdate(2);

            Item.ExtraData = "1";
            Item.UpdateState(false, true);
            Item.ReqUpdate(2);
        }

        public void Destroy()
        {
            if (this.banzaiTeleports != null)
                this.banzaiTeleports.Clear();
            if (this.banzaiPyramids != null)
                this.banzaiPyramids.Clear();
            this.banzaiPyramids = (Dictionary<int, Item>)null;
            this.banzaiTeleports = (Dictionary<int, Item>)null;
            this.room = (Room)null;
        }
    }
}
