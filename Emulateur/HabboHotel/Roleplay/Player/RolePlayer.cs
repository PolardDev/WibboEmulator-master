using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Roleplay.Weapon;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.HabboHotel.Roleplay.Player
{
    public class RolePlayer
    {
        private readonly int _rpId;
        private readonly int _id;

        private readonly ConcurrentDictionary<int, RolePlayInventoryItem> _inventory;

        public int Health { get; set; }
        public int HealthMax { get; set; }
        public int Money { get; set; }
        public int Munition { get; set; }
        public int GunLoad { get; set; }
        public int Exp { get; set; }
        public bool Dead { get; set; }
        public bool SendPrison { get; set; }
        public int Level { get; set; }
        public RPWeapon WeaponGun { get; set; }
        public RPWeapon WeaponCac { get; set; }
        public int Energy { get; set; }
        public int GunLoadTimer { get; set; }
        public int PrisonTimer { get; set; }
        public int DeadTimer { get; set; }
        public int SlowTimer { get; private set; }
        public int AggroTimer { get; set; }
        public bool PvpEnable { get; set; }
        
        public int TradeId { get; set; }
        public bool NeedUpdate { get; private set; }
        public bool Dispose { get; private set; }

        public RolePlayer(int pRpId, int pId, int pHealth, int pMoney, int pMunition, int pExp, int pEnergy, int pWeaponGun, int pWeaponCac)
        {
            this._rpId = pRpId;
            this._id = pId;
            this.Health = pHealth;
            this.Energy = pEnergy;
            this.Money = pMoney;
            this.Munition = pMunition;
            this.Exp = pExp;
            this.PvpEnable = true;
            this.WeaponCac = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(pWeaponCac);
            this.WeaponGun = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(pWeaponGun);

            this.GunLoad = 6;
            this.GunLoadTimer = 0;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {

                int expmax = (i * 50) + (i * 10) * i;

                if (Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }
            this.Level = Level;
            this.HealthMax = 90 + (this.Level * 10);

            this.SendPrison = false;
            this.PrisonTimer = 0;
            this.Dead = false;
            this.DeadTimer = 0;

            this.AggroTimer = 0;
            this.SlowTimer = 0;

            this._inventory = new ConcurrentDictionary<int, RolePlayInventoryItem>();
            //this.LoadInventory();

            this.TradeId = 0;
        }

        public void LoadInventory()
        {
            DataTable Table;
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");

                queryreactor.AddParameter("userid", this._id);
                Table = queryreactor.GetTable();
            }
            foreach (DataRow dataRow in Table.Rows)
            {
                if (!this._inventory.ContainsKey((int)dataRow["item_id"]))
                    this._inventory.TryAdd((int)dataRow["item_id"], new RolePlayInventoryItem((int)dataRow["id"], (int)dataRow["item_id"], (int)dataRow["count"]));
            }

           
            this.SendWebPacket(new LoadInventoryRpComposer(this._inventory));
        }

        internal RolePlayInventoryItem GetInventoryItem(int Id)
        {
            RolePlayInventoryItem Item = null;
            this._inventory.TryGetValue(Id, out Item);
            return Item;
        }

        internal void AddInventoryItem(int pItemId, int pCount = 1)
        {
            RPItem RPItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(pItemId);
            if (RPItem == null)
                return;

            RolePlayInventoryItem Item = GetInventoryItem(pItemId);
            if (Item == null)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_rpitems` (`user_id`, `rp_id`, `item_id`, `count`) VALUES ('" + this._id + "', '" + this._rpId + "', '" + pItemId + "', '"+ pCount + "')");
                    int Id = Convert.ToInt32(dbClient.InsertQuery());
                    this._inventory.TryAdd(pItemId, new RolePlayInventoryItem(Id, pItemId, pCount));
                }
            }
            else
            {
                Item.Count += pCount;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("UPDATE user_rpitems SET count = count + '"+ pCount + "' WHERE id = '" + Item.Id + "' LIMIT 1");
            }

            
            this.SendWebPacket(new AddInventoryItemRpComposer(RPItem, pCount));
        }

        internal void RemoveInventoryItem(int ItemId, int Count = 1)
        {
            RolePlayInventoryItem Item = GetInventoryItem(ItemId);
            if (Item == null)
                return;

            if (Item.Count > Count)
            {
                Item.Count = Item.Count - Count;

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("UPDATE user_rpitems SET count = count - '" + Count + "' WHERE id = '" + Item.Id + "' LIMIT 1");
            }
            else
            {
                this._inventory.TryRemove(ItemId, out Item);

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("DELETE FROM user_rpitems WHERE id = '" + Item.Id + "' LIMIT 1");
            }
            
             this.SendWebPacket(new RemoveItemInventoryRpComposer(ItemId, Count));
        }

        public void SendWebPacket(IServerPacket Message)
        {
            WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(this._id);
            if (ClientWeb != null)
            {
                ClientWeb.SendPacket(Message);
            }
        }

        internal void RemoveMunition(int Nb)
        {
            if (this.Munition - Nb <= 0)
                this.Munition = 0;
            else
                this.Munition -= Nb;
        }

        internal void AddMunition(int Nb)
        {
            if (Nb <= 0)
                return;

            if (Nb > 99)
                Nb = 99;

            if (this.Munition + Nb > 99)
                this.Munition = 99;
            else
                this.Munition += Nb;
        }

        internal void AddHealth(int Nb)
        {
            if (Nb <= 0)
                return;

            if (this.Health + Nb > this.HealthMax)
                this.Health = this.HealthMax;
            else
                this.Health += Nb;
        }

        public void AddExp(int pNb)
        {
            this.Exp += pNb;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {
                int expmax = (i * 50) + (i * 10) * i;

                if (Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }

            if (this.Level < Level)
            {
                this.Level = Level;
                this.HealthMax = 90 + (this.Level * 10);
                this.Health = this.HealthMax;
                this.SendUpdate();
            }
        }



        public void RemoveExp(int pNb)
        {
            if (this.Exp >= pNb)
                this.Exp -= pNb;
            else
                this.Exp = 0;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {
                int expmax = (i * 50) + (i * 10) * i;

                if (Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }

            if (this.Level != Level)
            {
                this.Level = Level;
                this.HealthMax = 90 + (this.Level * 10);
                this.Health = this.HealthMax;
                this.SendUpdate();
            }
        }

        internal void AddEnergy(int Nb)
        {
            if (this.Energy + Nb > 100)
                this.Energy = 100;
            else
                this.Energy += Nb;
        }

        internal void RemoveEnergy(int Nb)
        {
            if (this.Energy - Nb < 0)
                this.Energy = 0;
            else
                this.Energy -= Nb;
        }

        public void Hit(RoomUser User, int Dmg, Room Room, bool Ralentie = false, bool Murmur = false, bool Aggro = true)
        {
            if (this.Dead || this.SendPrison)
                return;

            if (this.Health <= Dmg)
            {
                this.Health = 0;
                this.Dead = true;
                this.DeadTimer = 30;

                User.SetStatus("lay", "0.7");
                User.Freeze = true;
                User.FreezeEndCounter = 0;
                User.IsLay = true;
                User.UpdateNeeded = true;

                if(User.GetClient() != null)
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.userdead", User.GetClient().Langue));

                int monaiePerdu = 0;
                if (this.Money > 10)
                {
                    monaiePerdu = (int)Math.Floor((double)(this.Money / 100) * 20);
                    this.Money -= monaiePerdu;
                }
                Room.GetRoomItemHandler().AddTempItem(User.VirtualId, 5461, User.SetX, User.SetY, User.Z, "1", monaiePerdu + 10, InteractionTypeTemp.MONEY);
                User.OnChat("A été mis K.O. ! [" + this.Health + "/" + this.HealthMax + "]", 0, true);
            }
            else
            {
                this.Health -= Dmg;
                if (Ralentie)
                {
                    if (this.SlowTimer == 0)
                    {
                        if (User.GetClient() != null)
                            User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.hitslow", User.GetClient().Langue));
                    }
                    this.SlowTimer = 6;
                }

                if(Aggro)
                    this.AggroTimer = 30;

                if (User.GetClient() != null)
                {
                    if(Murmur)
                        User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), false);
                    else
                        User.OnChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), 0, true);
                }
            }

            this.SendUpdate();
        }

        public void SendUpdate(bool SendNow = false)
        {
            if(SendNow)
                this.SendWebPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
            else
                this.NeedUpdate = true;
        }
        
        public void SendItemsList(List<RPItem> ItemsList)
        {
            this.SendWebPacket(new BuyItemsListComposer(ItemsList));
        }

        public void OnCycle(RoomUser User, RolePlayerManager RPManager)
        {
            /*this._energyTimer--;
            if(this._energyTimer <= 0)
            {
                this._energyTimer = 120;
                if (this._energy > 0)
                {
                    this._energy--;
                    this.SendUpdate();
                }
            }*/

            if (this.SlowTimer > 0)
            {
                this.SlowTimer--;
                User.breakwalk = true;
            } else
            {
                User.breakwalk = false;
            }

            if(this.GunLoadTimer > 0)
            {
                this.GunLoadTimer--;
                if(this.GunLoadTimer == 0)
                {
                    this.GunLoad = 6;
                }
            } else
            {
                if(this.GunLoad == 0)
                {
                    this.GunLoadTimer = 6;
                    User.OnChat("*Recharge mon arme*");
                }
            }


            if(this.AggroTimer > 0)
                this.AggroTimer--;

            if (this.SendPrison)
            {
                if (this.PrisonTimer > 0)
                    this.PrisonTimer--;
                else
                {
                    this.SendPrison = false;
                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = RPManager.PrisonId;
                    User.GetClient().GetHabbo().PrepareRoom(RPManager.PrisonId);
                }
            }

            if (this.Dead)
            {
                if (this.DeadTimer > 0)
                    this.DeadTimer--;
                else
                {
                    this.Dead = false;
                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = RPManager.HopitalId;
                    User.GetClient().GetHabbo().PrepareRoom(RPManager.HopitalId);
                }
            }

            if(this.NeedUpdate)
            {
                this.NeedUpdate = false;
                this.SendWebPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
            }
        }

        public void Destroy()
        {
            if (this.Dispose)
                return;

            this.Dispose = true;
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE `user_rp` SET `health`='" + this.Health + "', `energy`='" + this.Energy + "' , `money`='" + this.Money + "', `munition`='" + this.Munition + "', `exp`='" + this.Exp + "', `weapon_far`='" + this.WeaponGun.Id + "', `weapon_cac`='" + this.WeaponCac.Id + "' WHERE `user_id`='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
            }

            this.SendWebPacket(new RpStatsComposer(0, 0, 0, 0, 0, 0, 0));
            this._inventory.Clear();
        }
    }
}