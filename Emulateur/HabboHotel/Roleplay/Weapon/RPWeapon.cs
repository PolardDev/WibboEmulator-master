namespace Butterfly.HabboHotel.Roleplay.Weapon
{
    public class RPWeapon
    {
        public int Id { get; set; }
        public int DmgMin { get; set; }
        public int DmgMax { get; set; }
        public RPWeaponInteraction Interaction { get; set; }
        public int Enable { get; }
        public int FreezeTime { get; }
        public int Distance { get; }

        public RPWeapon(int pId, int pDmgMin, int pDmgMax, RPWeaponInteraction pInteraction, int pEnable, int pFreezeTime, int pDistance)
        {
            this.Id = pId;
            this.DmgMin = pDmgMin;
            this.DmgMax = pDmgMax;
            this.Interaction = pInteraction;
            this.Enable = pEnable;
            this.FreezeTime = pFreezeTime;
            this.Distance = pDistance;
        }
    }
}
