namespace Butterfly.HabboHotel.Roleplay.Player
{
    public class RolePlayInventoryItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }

        public RolePlayInventoryItem(int pId, int pItemId, int pCount)
        {
            this.Id = pId;
            this.ItemId = pItemId;
            this.Count = pCount;
        }
    }
}
