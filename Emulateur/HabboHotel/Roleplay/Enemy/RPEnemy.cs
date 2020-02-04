namespace Butterfly.HabboHotel.Roleplay.Enemy
{
    public class RPEnemy
    {
        public int Id { get; set; }
        public int Health { get; set; }
        public int WeaponGunId { get; set; }
        public int WeaponCacId { get; set; }
        public int DeadTimer { get; set; }
        public int MoneyDrop { get; set; }
        public int DropScriptId { get; set; }
        public int TeamId { get; set; }
        public int ZoneDistance { get; set; }
        public bool ResetPosition { get; set; }
        public int AggroDistance { get; set; }
        public int LootItemId { get; set; }
        public int LostAggroDistance { get; set; }
        public bool ZombieMode { get; set; }

        public RPEnemy(int pId, int pHealth, int pWeaponGunId, int pWeaponCacId, int pDeadTimer, int pLootItemId, int pMoneyDrop, int pDropScriptId, int pTeamId, int pAggroDistance, int pZoneDistance,
            bool pResetPosition, int pLostAggroDistance, bool pZombieMode)
        {
            this.Id = pId;
            this.Health = pHealth;
            this.WeaponGunId = pWeaponGunId;
            this.WeaponCacId = pWeaponCacId;
            this.DeadTimer = pDeadTimer;
            this.MoneyDrop = pMoneyDrop;
            this.LootItemId = pLootItemId;
            this.MoneyDrop = pMoneyDrop;
            this.DropScriptId = pDropScriptId;
            this.TeamId = pTeamId;
            this.AggroDistance = pAggroDistance;
            this.ZoneDistance = pZoneDistance;
            this.ResetPosition = pResetPosition;
            this.LostAggroDistance = pLostAggroDistance;
            this.ZombieMode = pZombieMode;
        }
    }
}
