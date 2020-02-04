using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Wired
{
    public class WiredCycle
    {
        public RoomUser User;
        public Item Item;
        public IWiredCycleable IWiredCycleable;
        public int Cycle;
        //public int Delay;

        public WiredCycle(IWiredCycleable pIWiredCycleable, RoomUser pUser, Item pItem, int pDelay)
        {
            this.IWiredCycleable = pIWiredCycleable;
            this.User = pUser;
            this.Item = pItem;
            this.Cycle = 0;
            //this.Delay = pDelay;
        }

        public bool OnCycle()
        {
            this.Cycle++;

            if (this.Cycle <= this.IWiredCycleable.Delay)
                return true;

            this.Cycle = 0;
            
            if (this.User == null || (this.User != null && this.User.mDispose))
                this.User = null;
            
            return this.IWiredCycleable.OnCycle(this.User, this.Item);
        }
    }
}
