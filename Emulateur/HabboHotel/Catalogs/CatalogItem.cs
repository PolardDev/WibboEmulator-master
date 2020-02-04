using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Catalog
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ItemData Data { get; set; }
        public int Amount { get; set; }
        public int CostCredits { get; set; }
        public bool HaveOffer { get; set; }
        public bool IsLimited { get; set; }
        public string Name { get; set; }
        public int PageID { get; set; }
        public int CostDuckets { get; set; }
        public int LimitedEditionStack { get; set; }
        public int LimitedEditionSells { get; set; }
        public int CostDiamonds { get; set; }
        public string Badge { get; set; }

        public CatalogItem(int Id, int ItemId, ItemData Data, string CatalogName, int PageId, int CostCredits, int CostPixels,
            int CostDiamonds, int Amount, int LimitedEditionSells, int LimitedEditionStack, bool HaveOffer, string Badge)
        {
            this.Id = Id;
            this.Name = CatalogName;
            this.ItemId = ItemId;
            this.Data = Data;
            this.PageID = PageId;
            this.CostCredits = CostCredits;
            this.CostDuckets = CostPixels;
            this.CostDiamonds = CostDiamonds;
            this.Amount = Amount;
            this.LimitedEditionSells = LimitedEditionSells;
            this.LimitedEditionStack = LimitedEditionStack;
            this.IsLimited = (LimitedEditionStack > 0);
            this.HaveOffer = HaveOffer;
            this.Badge = Badge;
        }
    }
}