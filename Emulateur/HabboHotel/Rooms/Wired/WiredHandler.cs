using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Concurrent;

namespace Butterfly.HabboHotel.Rooms.Wired
{
    public class WiredHandler
    {
        private readonly ConcurrentDictionary<Point, List<Item>> actionStacks;
        private readonly ConcurrentDictionary<Point, List<Item>> conditionStacks;

        private readonly ConcurrentDictionary<Point, List<RoomUser>> WiredUsed;

        private readonly List<Point> SpecialRandom;
        private readonly Dictionary<Point, int> SpecialUnseen;

        public event BotCollisionDelegate TrgBotCollision;
        public event UserAndItemDelegate TrgCollision;
        public event RoomEventDelegate TrgTimer;

        private ConcurrentQueue<WiredCycle> requestingUpdates;

        private readonly Room room;
        private bool doCleanup = false;

        public WiredHandler(Room room)
        {
            this.actionStacks = new ConcurrentDictionary<Point, List<Item>>();
            this.conditionStacks = new ConcurrentDictionary<Point, List<Item>>();
            this.requestingUpdates = new ConcurrentQueue<WiredCycle>();
            this.WiredUsed = new ConcurrentDictionary<Point, List<RoomUser>>();


            this.SpecialRandom = new List<Point>();
            this.SpecialUnseen = new Dictionary<Point, int>();

            this.room = room;
        }

        public void AddFurniture(Item item)
        {
            Point itemCoord = item.Coordinate;
            if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
            {
                if (this.actionStacks.ContainsKey(itemCoord))
                {
                    ((List<Item>)this.actionStacks[itemCoord]).Add(item);
                }
                else
                {
                    this.actionStacks.TryAdd(itemCoord, new List<Item>() { item });
                }
            }
            else if (WiredUtillity.TypeIsWiredCondition(item.GetBaseItem().InteractionType))
            {
                if (this.conditionStacks.ContainsKey(itemCoord))
                {
                    ((List<Item>)this.conditionStacks[itemCoord]).Add(item);
                }
                else
                {
                    this.conditionStacks.TryAdd(itemCoord, new List<Item>() { item });
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.specialrandom)
            {
                if (!this.SpecialRandom.Contains(itemCoord))
                {
                    this.SpecialRandom.Add(itemCoord);
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.specialunseen)
            {
                if (!this.SpecialUnseen.ContainsKey(itemCoord))
                {
                    this.SpecialUnseen.Add(itemCoord, 0);
                }
            }
        }

        public void RemoveFurniture(Item item)
        {
            Point itemCoord = item.Coordinate;
            if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
            {
                Point coordinate = item.Coordinate;
                if (!this.actionStacks.ContainsKey(coordinate))
                    return;

                ((List<Item>)this.actionStacks[coordinate]).Remove(item);
                if (this.actionStacks[coordinate].Count == 0)
                {
                    List<Item> NewList = new List<Item>();
                    this.actionStacks.TryRemove(coordinate, out NewList);
                }
            }
            else if (WiredUtillity.TypeIsWiredCondition(item.GetBaseItem().InteractionType))
            {
                if (!this.conditionStacks.ContainsKey(itemCoord))
                    return;

                ((List<Item>)this.conditionStacks[itemCoord]).Remove(item);
                if (this.conditionStacks[itemCoord].Count == 0)
                {
                    List<Item> NewList = new List<Item>();
                    this.conditionStacks.TryRemove(itemCoord, out NewList);
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.specialrandom)
            {
                if (this.SpecialRandom.Contains(itemCoord))
                {
                    this.SpecialRandom.Remove(itemCoord);
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.specialunseen)
            {
                if (this.SpecialUnseen.ContainsKey(itemCoord))
                {
                    this.SpecialUnseen.Remove(itemCoord);
                }
            }
        }

        public void OnCycle()
        {
            if (this.doCleanup)
                ClearWired();
            else
            {
                if (this.requestingUpdates.Count > 0)
                {
                    List<WiredCycle> toAdd = new List<WiredCycle>();
                    while (this.requestingUpdates.Count > 0)
                    {
                        WiredCycle handler = null;
                        if (!this.requestingUpdates.TryDequeue(out handler))
                            continue;

                        if (handler.IWiredCycleable.Disposed())
                            continue;
                        
                        if (handler.OnCycle())
                            toAdd.Add(handler);
                    }

                    foreach (WiredCycle cycle in toAdd)
                        this.requestingUpdates.Enqueue(cycle);
                }

                this.WiredUsed.Clear();
            }
        }

        private void ClearWired()
        {
            foreach (List<Item> list in this.actionStacks.Values)
            {
                foreach (Item roomItem in list)
                {
                    if (roomItem.WiredHandler != null)
                    {
                        roomItem.WiredHandler.Dispose();
                        roomItem.WiredHandler = null;
                    }
                }
            }
            foreach (List<Item> list in this.conditionStacks.Values)
            {
                foreach (Item roomItem in list)
                {
                    if (roomItem.WiredHandler != null)
                    {
                        roomItem.WiredHandler.Dispose();
                        roomItem.WiredHandler = null;
                    }
                }
            }
            this.conditionStacks.Clear();
            this.actionStacks.Clear();
            this.WiredUsed.Clear();
            this.doCleanup = false;
        }

        public void OnPickall()
        {
            this.doCleanup = true;
        }

        public void ExecutePile(Point coordinate, RoomUser user, Item item)
        {
            if (!this.actionStacks.ContainsKey(coordinate))
                return;

            if (this.WiredUsed.ContainsKey(coordinate))
            {
                if (this.WiredUsed[coordinate].Contains(user))
                    return;
                else
                {
                    this.WiredUsed[coordinate].Add(user);
                }
            }
            else
            {
                this.WiredUsed.TryAdd(coordinate, new List<RoomUser>() { user });
            }

            if (this.conditionStacks.ContainsKey(coordinate))
            {
                List<Item> ConditionStack = this.conditionStacks[coordinate];
                int CycleCountCdt = 0;
                foreach (Item roomItem in ConditionStack.ToArray())
                {
                    CycleCountCdt++;
                    if (CycleCountCdt > 20)
                        break;
                    if (roomItem == null || roomItem.WiredHandler == null)
                        continue;
                    if (!((IWiredCondition)roomItem.WiredHandler).AllowsExecution(user, item))
                        return;
                }
            }

            List<Item> ActionStack = (List<Item>)this.actionStacks[coordinate];

            if (this.SpecialRandom.Contains(coordinate))
            {
                int CountAct = ActionStack.Count - 1;

                int RdnWired = ButterflyEnvironment.GetRandomNumber(0, CountAct);
                Item ActRand = ActionStack[RdnWired];
                ((IWiredEffect)ActRand.WiredHandler).Handle(user, item);
            }
            else if (this.SpecialUnseen.ContainsKey(coordinate))
            {
                int CountAct = ActionStack.Count - 1;

                int NextWired = this.SpecialUnseen[coordinate];
                if (NextWired > CountAct)
                {
                    NextWired = 0;
                    this.SpecialUnseen[coordinate] = 0;
                }
                this.SpecialUnseen[coordinate]++;

                Item ActNext = ActionStack[NextWired];
                if (ActNext != null && ActNext.WiredHandler != null)
                {
                    ((IWiredEffect)ActNext.WiredHandler).Handle(user, item);
                }
            }
            else
            {
                int CycleCount = 0;
                foreach (Item roomItem in ActionStack.ToArray())
                {
                    CycleCount++;

                    if (CycleCount > 20)
                        break;
                    if (roomItem != null && roomItem.WiredHandler != null)
                        ((IWiredEffect)roomItem.WiredHandler).Handle(user, item);
                }
            }
        }

        public void RequestCycle(WiredCycle handler)
        {
            this.requestingUpdates.Enqueue(handler);
        }

        public Room GetRoom()
        {
            return this.room;
        }

        public void Destroy()
        {
            if (this.actionStacks != null)
                this.actionStacks.Clear();

            if (this.conditionStacks != null)
                this.conditionStacks.Clear();

            if (this.requestingUpdates != null)
                this.requestingUpdates = null;

            this.TrgCollision = null;
            this.TrgBotCollision = null;
            this.TrgTimer = null;
            this.WiredUsed.Clear();
        }

        public void TriggerCollision(RoomUser roomUser, Item Item)
        {
            if (this.TrgCollision != null)
                this.TrgCollision(roomUser, Item);
        }

        public void TriggerBotCollision(RoomUser roomUser, string BotName)
        {
            if (this.TrgBotCollision != null)
                this.TrgBotCollision(roomUser, BotName);
        }

        public void TriggerTimer()
        {
            if (this.TrgTimer != null)
                this.TrgTimer(null, null);
        }
    }
}
