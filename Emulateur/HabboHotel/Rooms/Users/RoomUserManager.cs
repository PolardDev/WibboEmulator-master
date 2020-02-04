using Butterfly.Core;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Pathfinding;
using Butterfly.HabboHotel.Pets;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.RoomBots;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Utilities;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Collections.Concurrent;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Roleplay.Enemy;
using Butterfly.HabboHotel.Rooms.Map.Movement;

namespace Butterfly.HabboHotel.Rooms
{
    public delegate void UserAndItemDelegate(RoomUser user, Item item);

    public class RoomUserManager
    {
        private Room room;
        public ConcurrentDictionary<string, RoomUser> usersByUsername;
        public ConcurrentDictionary<int, RoomUser> usersByUserID;

        private ConcurrentDictionary<int, RoomUser> _users;
        private readonly ConcurrentDictionary<int, RoomUser> _pets;
        private readonly ConcurrentDictionary<int, RoomUser> _bots;

        private readonly List<int> _usersRank;

        public int BotCount;

        private int primaryPrivateUserID;



        public event RoomEventDelegate OnUserEnter;

        public RoomUserManager(Room room)
        {
            this.room = room;
            this._users = new ConcurrentDictionary<int, RoomUser>();
            this._pets = new ConcurrentDictionary<int, RoomUser>();
            this._bots = new ConcurrentDictionary<int, RoomUser>();
            this.usersByUsername = new ConcurrentDictionary<string, RoomUser>();
            this.usersByUserID = new ConcurrentDictionary<int, RoomUser>();
            this._usersRank = new List<int>();
            this.primaryPrivateUserID = 1;
            this.BotCount = 0;
        }

        public void UserEnter(RoomUser thisUser)
        {
            if (this.OnUserEnter != null)
                this.OnUserEnter(thisUser, (EventArgs)null);
        }

        public int GetRoomUserCount()
        {
            return this.room.RoomData.UsersNow;
        }

        public RoomUser DeploySuperBot(RoomBot Bot)
        {
            int key = this.primaryPrivateUserID++;
            RoomUser roomUser = new RoomUser(0, this.room.Id, key, this.room);

            this._users.TryAdd(key, roomUser);


            roomUser.SetPos(Bot.X, Bot.Y, Bot.Z);
            roomUser.SetRot(Bot.Rot, false);

            roomUser.BotData = Bot;
            roomUser.BotAI = Bot.GenerateBotAI(roomUser.VirtualId);

            roomUser.BotAI.Init(Bot.Id, roomUser.VirtualId, this.room.Id, roomUser, this.room);

            this.BotCount++;
            roomUser.SetStatus("flatctrl 4", "");
            this.UpdateUserStatus(roomUser, false);
            roomUser.UpdateNeeded = true;

            this.room.SendPacket(new UsersComposer(roomUser));

            roomUser.BotAI.OnSelfEnterRoom();

            if (this._bots.ContainsKey(roomUser.BotData.Id))
                this._bots[roomUser.BotData.Id] = roomUser;
            else
                this._bots.TryAdd(roomUser.BotData.Id, roomUser);

            return roomUser;
        }

        public bool UpdateClientUsername(RoomUser User, string OldUsername, string NewUsername)
        {
            if (!this.usersByUsername.ContainsKey(OldUsername.ToLower()))
                return false;
            usersByUsername.TryRemove(OldUsername.ToLower(), out User);
            usersByUsername.TryAdd(NewUsername.ToLower(), User);
            return true;
        }

        public RoomUser DeployBot(RoomBot Bot, Pet PetData)
        {
            int key = this.primaryPrivateUserID++;
            RoomUser roomUser = new RoomUser(0, this.room.Id, key, this.room);

            this._users.TryAdd(key, roomUser);

            roomUser.SetPos(Bot.X, Bot.Y, Bot.Z);
            roomUser.SetRot(Bot.Rot, false);

            roomUser.BotData = Bot;

            if (this.room.RpRoom)
            {
                RPEnemy Enemy = null;
                if (Bot.IsPet)
                    Enemy = ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(Bot.Id);
                else
                    Enemy = ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(Bot.Id);

                if (Enemy != null)
                {
                    roomUser.BotData.RoleBot = new RoleBot(Enemy);
                    if (Bot.IsPet)
                        roomUser.BotData.AiType = AIType.RolePlayPet;
                    else
                        roomUser.BotData.AiType = AIType.RolePlayBot;
                }
            }

            roomUser.BotAI = Bot.GenerateBotAI(roomUser.VirtualId);

            if (roomUser.IsPet)
            {
                roomUser.BotAI.Init(Bot.Id, roomUser.VirtualId, this.room.Id, roomUser, this.room);
                roomUser.PetData = PetData;
                roomUser.PetData.VirtualId = roomUser.VirtualId;
            }
            else
            {
                roomUser.BotAI.Init(Bot.Id, roomUser.VirtualId, this.room.Id, roomUser, this.room);
            }
            this.BotCount++;
            roomUser.SetStatus("flatctrl 4", "");

            if (Bot.Status == 1)
            {
                roomUser.SetStatus("sit", "0.5");
                roomUser.IsSit = true;
            }

            if (Bot.Status == 2)
            {
                roomUser.SetStatus("lay", "0.7");
                roomUser.IsLay = true;
            }

            this.UpdateUserStatus(roomUser, false);
            roomUser.UpdateNeeded = true;

            if (Bot.IsDancing)
            {
                roomUser.DanceId = 3;
                ServerPacket Response = new ServerPacket(ServerPacketHeader.DanceMessageComposer);
                Response.WriteInteger(roomUser.VirtualId);
                Response.WriteInteger(3);
                this.room.SendPacket(Response);
            }

            if (Bot.Enable > 0)
            {
                roomUser.ApplyEffect(Bot.Enable);
            }

            if(Bot.Handitem > 0)
            {
                roomUser.CarryItem(Bot.Handitem, true);
            }

            this.room.SendPacket(new UsersComposer(roomUser));

            roomUser.BotAI.OnSelfEnterRoom();
            if (roomUser.IsPet)
            {
                if (this._pets.ContainsKey(roomUser.PetData.PetId))
                    this._pets[roomUser.PetData.PetId] = roomUser;
                else
                    this._pets.TryAdd(roomUser.PetData.PetId, roomUser);
            }
            else if (this._bots.ContainsKey(roomUser.BotData.Id))
                this._bots[roomUser.BotData.Id] = roomUser;
            else
                this._bots.TryAdd(roomUser.BotData.Id, roomUser);

            return roomUser;
        }

        public void RemoveBot(int VirtualId, bool Kicked)
        {
            RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(VirtualId);
            if (roomUserByVirtualId == null || !roomUserByVirtualId.IsBot)
                return;
            if (roomUserByVirtualId.IsPet)
            {
                RoomUser PetRemoval = null;
                this._pets.TryRemove(roomUserByVirtualId.PetData.PetId, out PetRemoval);
            }
            else
            {
                RoomUser BotRemoval = null;
                this._bots.TryRemove(roomUserByVirtualId.BotData.Id, out BotRemoval);
            }
            this.BotCount--;
            roomUserByVirtualId.BotAI.OnSelfLeaveRoom(Kicked);

            this.room.SendPacket(new UserRemoveComposer(roomUserByVirtualId.VirtualId));

            this.room.GetGameMap().RemoveTakingSquare(roomUserByVirtualId.SetX, roomUserByVirtualId.SetY);
            this.room.GetGameMap().RemoveUserFromMap(roomUserByVirtualId, new Point(roomUserByVirtualId.X, roomUserByVirtualId.Y));

            RoomUser toRemove = null;
            this._users.TryRemove(roomUserByVirtualId.VirtualId, out toRemove);

        }

        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            try
            {
                if (User == null)
                    return;
                if (User.IsPet)
                    return;
                if (!this.room.GetGameMap().ValidTile(x, y))
                    return;
                byte pByte = this.room.GetGameMap().EffectMap[x, y];
                if (pByte > 0)
                {
                    ItemEffectType itemEffectType = ByteToItemEffectEnum.Parse(pByte);
                    if (itemEffectType == User.CurrentItemEffect)
                        return;
                    switch (itemEffectType)
                    {
                        case ItemEffectType.None:
                            User.ApplyEffect(0);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.Swim:
                            User.ApplyEffect(29);
                            if (User.GetClient() != null)
                                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1948);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.SwimLow:
                            User.ApplyEffect(30);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.SwimHalloween:
                            User.ApplyEffect(37);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.Iceskates:
                            if (User.GetClient() != null)
                            {
                                if (User.GetClient().GetHabbo().Gender == "M")
                                    User.ApplyEffect(38);
                                else
                                    User.ApplyEffect(39);
                            }
                            else
                                User.ApplyEffect(38);
                            User.CurrentItemEffect = ItemEffectType.Iceskates;
                            if (User.GetClient() != null)
                                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1413);
                            break;
                        case ItemEffectType.Normalskates:
                            if (User.GetClient() != null)
                            {
                                if (User.GetClient().GetHabbo().Gender == "M")
                                    User.ApplyEffect(55);
                                else
                                    User.ApplyEffect(56);
                            }
                            else
                                User.ApplyEffect(55);

                            User.CurrentItemEffect = itemEffectType;
                            if (User.GetClient() != null)
                                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 2199);
                            break;
                        case ItemEffectType.Trampoline:
                            User.ApplyEffect(193);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.TreadMill:
                            User.ApplyEffect(194);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.CrossTrainer:
                            User.ApplyEffect(195);
                            User.CurrentItemEffect = itemEffectType;
                            break;

                    }
                }
                else
                {
                    if (User.CurrentItemEffect == ItemEffectType.None || pByte != 0)
                        return;
                    User.ApplyEffect(0);
                    User.CurrentItemEffect = ItemEffectType.None;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateUserEffecterreur: " + ex);
            }
        }

        public RoomUser GetUserForSquare(int x, int y)
        {
            return Enumerable.FirstOrDefault<RoomUser>((IEnumerable<RoomUser>)this.room.GetGameMap().GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot == true));
        }

        public RoomUser GetUserForSquareNotBot(int x, int y)
        {
            return Enumerable.FirstOrDefault<RoomUser>((IEnumerable<RoomUser>)this.room.GetGameMap().GetRoomUsers(new Point(x, y)).Where(u => u.IsBot == false));
        }

        public bool AddAvatarToRoom(GameClient Session)
        {
            if (room == null)
                return false;

            if (Session == null || Session.GetHabbo() == null)
                return false;

            int PersonalID = this.primaryPrivateUserID++;

            RoomUser User = new RoomUser(Session.GetHabbo().Id, this.room.Id, PersonalID, this.room);

            User.UserId = Session.GetHabbo().Id;
            User.IsSpectator = Session.GetHabbo().SpectatorMode;

            if (!this._users.TryAdd(PersonalID, User))
                return false;

            if (Session.GetHabbo().Rank > 5 && !this._usersRank.Contains(User.UserId))
                this._usersRank.Add(User.UserId);

            Session.GetHabbo().CurrentRoomId = this.room.Id;
            Session.GetHabbo().LoadingRoomId = 0;

            string Username = Session.GetHabbo().Username;
            int UserId = Session.GetHabbo().Id;

            if (this.usersByUsername.ContainsKey(Username.ToLower()))
                this.usersByUsername.TryRemove(Username.ToLower(), out User);

            if (this.usersByUserID.ContainsKey(UserId))
                this.usersByUserID.TryRemove(UserId, out User);

            this.usersByUsername.TryAdd(Username.ToLower(), User);
            this.usersByUserID.TryAdd(UserId, User);

            DynamicRoomModel Model = this.room.GetGameMap().Model;
            if (Model == null)
                return false;

            User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
            User.SetRot(Model.DoorOrientation, false);

            if (Session.GetHabbo().IsTeleporting)
            {
                Item roomItem = this.room.GetRoomItemHandler().GetItem(User.GetClient().GetHabbo().TeleporterId);
                if (roomItem != null)
                {
                    roomItem.GetRoom().GetGameMap().TeleportToItem(User, roomItem);

                    roomItem.InteractingUser2 = Session.GetHabbo().Id;
                    roomItem.ReqUpdate(1);
                }
            }

            if (User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                User.GetClient().GetHabbo().IsTeleporting = false;
                User.GetClient().GetHabbo().TeleporterId = 0;
                User.GetClient().GetHabbo().TeleportingRoomID = 0;
            }

            if (!User.IsSpectator)
                room.SendPacket(new UsersComposer(User));

            if (User.IsSpectator)
            {
                List<RoomUser> roomUserByRank = room.GetRoomUserManager().GetStaffRoomUser();
                if (roomUserByRank.Count > 0)
                {
                    foreach (RoomUser StaffUser in roomUserByRank)
                    {
                        if (StaffUser != null && StaffUser.GetClient() != null && (StaffUser.GetClient().GetHabbo() != null && StaffUser.GetClient().GetHabbo().HasFuse("fuse_sysadmin")))
                            StaffUser.SendWhisperChat(User.GetUsername() + " est entré dans l'appart en mode invisible !", true);
                    }
                }
            }

            if (room.CheckRights(Session, true))
            {
                User.SetStatus("flatctrl", "useradmin");
                Session.SendPacket(new YouAreOwnerComposer());
                Session.SendPacket(new YouAreControllerComposer(4));

                if (Session.GetHabbo().HasFuse("ads_background"))
                    Session.SendPacket(new UserRightsComposer(5));
            }
            else if (room.CheckRights(Session))
            {
                User.SetStatus("flatctrl", "1");
                Session.SendPacket(new YouAreControllerComposer(1));
            }
            else
            {
                if (Session.GetHabbo().HasFuse("ads_background"))
                    Session.SendPacket(new UserRightsComposer(Session.GetHabbo().Rank));
                Session.SendPacket(new YouAreNotControllerComposer());
            }

            if (!User.IsBot && Session.GetHabbo().Rank > 2)
            {
                if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ADM"))
                    User.CurrentEffect = 540;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("GPHWIB"))
                    User.CurrentEffect = 557;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur"))
                    User.CurrentEffect = 544;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("WIBARC"))
                    User.CurrentEffect = 546;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("CRPOFFI"))
                    User.CurrentEffect = 570;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ZEERSWS"))
                    User.CurrentEffect = 552;

                if (User.CurrentEffect > 0)
                    room.SendPacket(new AvatarEffectComposer(User.VirtualId, User.CurrentEffect));
            }

            User.UpdateNeeded = true;

            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                    continue;

                Bot.BotAI.OnUserEnterRoom(User);
            }

            if (!User.IsBot && this.room.RoomData.OwnerName != User.GetClient().GetHabbo().Username)
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.SOCIAL_VISIT, 0);

            if (!User.IsBot)
            {
                Session.GetHabbo().SendWebPacket(new InRoomComposer(true));

                if (Session.GetHabbo().RolePlayId > 0 && room.RoomData.OwnerId != Session.GetHabbo().RolePlayId)
                {
                    RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Session.GetHabbo().RolePlayId);
                    if (RPManager != null)
                    {
                        RolePlayer Rp = RPManager.GetPlayer(Session.GetHabbo().Id);
                        if (Rp != null)
                            RPManager.RemovePlayer(Session.GetHabbo().Id);

                    }
                    Session.GetHabbo().RolePlayId = 0;
                }

                if (room.RpRoom && room.RoomData.OwnerId != Session.GetHabbo().RolePlayId)
                {
                    RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(room.RoomData.OwnerId);
                    if (RPManager != null)
                    {
                        RolePlayer Rp = RPManager.GetPlayer(Session.GetHabbo().Id);
                        if (Rp == null)
                            RPManager.AddPlayer(Session.GetHabbo().Id);
                    }

                    Session.GetHabbo().RolePlayId = room.RoomData.OwnerId;
                }
            }


            User.InGame = room.RpRoom;

            return true;
        }

        public void RemoveUserFromRoom(GameClient Session, bool NotifyClient, bool NotifyKick)
        {
            try
            {
                if (Session == null)
                    return;

                if (Session.GetHabbo() == null)
                    return;

                if (NotifyClient)
                {
                    if (NotifyKick)
                    {
                        Session.SendPacket(new GenericErrorComposer(4008));
                    }
                    Session.SendPacket(new CloseConnectionComposer());
                }

                RoomUser User = this.GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (User == null)
                    return;

                if (this._usersRank.Contains(User.UserId))
                    this._usersRank.Remove(User.UserId);

                if (User.team != Team.none)
                {
                    this.room.GetTeamManager().OnUserLeave(User);
                    this.room.GetGameManager().UpdateGatesTeamCounts();
                }

                if (this.room.GotJanken())
                {
                    this.room.GetJanken().RemovePlayer(User);
                }

                if (User.RidingHorse)
                {
                    User.RidingHorse = false;
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(User.HorseID);
                    if (roomUserByVirtualId != null)
                    {
                        roomUserByVirtualId.RidingHorse = false;
                        roomUserByVirtualId.HorseID = 0;
                    }
                }

                if (User.IsSit || User.IsLay)
                {
                    User.IsSit = false;
                    User.IsLay = false;
                }

                if (this.room.HasActiveTrade(Session.GetHabbo().Id))
                    this.room.TryStopTrade(Session.GetHabbo().Id);

                if (User.Roleplayer != null)
                    ButterflyEnvironment.GetGame().GetRoleplayManager().GetTrocManager().RemoveTrade(User.Roleplayer.TradeId);

                if (User.IsSpectator)
                {
                    List<RoomUser> roomUserByRank = room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count > 0)
                    {
                        foreach (RoomUser StaffUser in roomUserByRank)
                        {
                            if (StaffUser != null && StaffUser.GetClient() != null && (StaffUser.GetClient().GetHabbo() != null && StaffUser.GetClient().GetHabbo().Rank >= 10))
                                StaffUser.SendWhisperChat(User.GetUsername() + " qui était en mode invisible est partie de l'appartement", true);
                        }
                    }
                }
                
                Session.GetHabbo().SendWebPacket(new InRoomComposer(false));

                Session.GetHabbo().CurrentRoomId = 0;
                Session.GetHabbo().LoadingRoomId = 0;

                this.usersByUserID.TryRemove(User.UserId, out User);
                this.usersByUsername.TryRemove(Session.GetHabbo().Username.ToLower(), out User);

                this.RemoveRoomUser(User);

                User.Freeze = true;
                User.FreezeEndCounter = 0;
                User.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogCriticalException("Error during removing user (" + Session.ConnectionID + ") from room:" + (ex).ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            this.room.GetGameMap().RemoveTakingSquare(user.SetX, user.SetY);
            this.room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));

            this.room.SendPacket(new UserRemoveComposer(user.VirtualId));
            
            this._users.TryRemove(user.VirtualId, out RoomUser toRemove);
        }

        public void UpdateUserCount(int count)
        {
            if (this.room.RoomData.UsersNow == count)
                return;

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery(string.Concat(new object[4]
                    {
                       "UPDATE rooms SET users_now = ",
                       count,
                       " WHERE id = ",
                       this.room.Id
             }));
            this.room.RoomData.UsersNow = count;
        }

        public RoomUser GetRoomUserByVirtualId(int VirtualId)
        {
            RoomUser User = null;
            if (!_users.TryGetValue(VirtualId, out User))
                return null;
            return User;
        }

        public RoomUser GetRoomUserByHabboId(int pId)
        {
            if (this.usersByUserID.ContainsKey(pId))
                return (RoomUser)this.usersByUserID[pId];
            else
                return (RoomUser)null;
        }

        public List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> List = new List<RoomUser>();

            List = this.GetUserList().Where(x => (!x.IsBot)).ToList();

            return List;
        }

        public ICollection<RoomUser> GetUserList()
        {
            return this._users.Values;
        }

        public RoomUser GetBotByName(string name)
        {
            return this._bots.Values.Where(b => b.IsBot && b.BotData.Name == name).FirstOrDefault();
        }

        public RoomUser GetBotOrPetByName(string name)
        {
            return this._bots.Values.Concat(this._pets.Values).Where(b => (b.IsBot && b.BotData.Name == name) || (b.IsPet && b.BotData.Name == name)).FirstOrDefault();
        }


        public List<RoomUser> GetStaffRoomUser()
        {
            List<RoomUser> list = new List<RoomUser>();
            foreach (int UserId in this._usersRank)
            {
                RoomUser roomUser = this.GetRoomUserByHabboId(UserId);
                if (roomUser != null)
                    list.Add(roomUser);
            }
            return list;
        }

        public RoomUser GetRoomUserByHabbo(string pName)
        {
            if (this.usersByUsername.ContainsKey(pName.ToLower()))
                return (RoomUser)this.usersByUsername[pName.ToLower()];
            else
                return (RoomUser)null;
        }

        public void SavePositionBots(IQueryAdapter dbClient)
        {
            List<RoomUser> Botlist = this.GetBots();
            if (Botlist.Count <= 0)
                return;

            QueryChunk queryChunk = new QueryChunk();

            foreach (RoomUser bot in Botlist)
            {
                RoomBot BotData = bot.BotData;
                if (BotData.AiType == AIType.RolePlayBot)
                    continue;
                if (bot.X != BotData.X || bot.Y != BotData.Y || bot.Z != BotData.Z || bot.RotBody != BotData.Rot)
                    queryChunk.AddQuery("UPDATE bots SET x = '" + bot.X + "', y = '" + bot.Y + "', z = '" + bot.Z + "', rotation = '" + bot.RotBody + "' WHERE id = " + bot.BotData.Id);
            }

            queryChunk.Execute(dbClient);
            queryChunk.Dispose();
        }

        public void AppendPetsUpdateString(IQueryAdapter dbClient)
        {
            List<RoomUser> Petlist = this.GetPets();
            if (Petlist.Count <= 0)
                return;
            QueryChunk queryChunk = new QueryChunk();
            QueryChunk queryChunk2 = new QueryChunk();

            foreach (RoomUser petData in Petlist)
            {
                Pet pet = petData.PetData;
                if (pet.DBState == DatabaseUpdateState.NeedsUpdate)
                {
                    queryChunk.AddParameter(pet.PetId + "name", pet.Name);
                    queryChunk.AddParameter(pet.PetId + "race", pet.Race);
                    queryChunk.AddParameter(pet.PetId + "color", pet.Color);
                    queryChunk.AddQuery("UPDATE user_pets SET room_id = " + pet.RoomId + ", name = @" + pet.PetId + "name, race = @" + pet.PetId + "race, color = @" + pet.PetId + "color, type = " + pet.Type + ", expirience = " + pet.Expirience + ", energy = " + pet.Energy + ", nutrition = " + pet.Nutrition + ", respect = " + pet.Respect + ", createstamp = '" + pet.CreationStamp + "', x = " + petData.X + ", Y = " + petData.Y + ", Z = " + petData.Z + " WHERE id = " + pet.PetId);
                }
                else
                {
                    if (petData.BotData.AiType == AIType.RolePlayPet)
                        continue;
                    queryChunk2.AddQuery("UPDATE user_pets SET x = " + petData.X + ", Y = " + petData.Y + ", Z = " + petData.Z + " WHERE id = " + pet.PetId);
                }
                pet.DBState = DatabaseUpdateState.Updated;
            }
            queryChunk.Execute(dbClient);
            queryChunk2.Execute(dbClient);
            queryChunk.Dispose();
            queryChunk2.Dispose();
        }

        public List<RoomUser> GetBots()
        {
            List<RoomUser> Bots = new List<RoomUser>();
            foreach (RoomUser User in this._bots.Values.ToList())
            {
                if (User == null || !User.IsBot || User.IsPet)
                    continue;

                Bots.Add(User);
            }

            return Bots;
        }

        public List<RoomUser> GetPets()
        {
            List<RoomUser> Pets = new List<RoomUser>();
            foreach (RoomUser User in this._pets.Values.ToList())
            {
                if (User == null || !User.IsPet)
                    continue;

                Pets.Add(User);
            }

            return Pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> Users = new List<RoomUser>();
            ICollection<RoomUser> RoomUsers = GetUserList();

            if (RoomUsers == null)
                return;

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null || !User.UpdateNeeded)
                    continue;

                User.UpdateNeeded = false;
                Users.Add(User);
            }

            this.room.SendPacket(new UserUpdateComposer(Users));
        }

        public void UpdateUserStatusses()
        {
            this.onUserUpdateStatus();
        }

        private void onUserUpdateStatus()
        {
            foreach (RoomUser User in this.GetUserList().ToList())
                this.UpdateUserStatus(User, false);
        }

        private bool isValid(RoomUser user)
        {
            return user.IsBot || user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().CurrentRoomId == this.room.Id;
        }

        public bool TryGetPet(int PetId, out RoomUser Pet)
        {
            return this._pets.TryGetValue(PetId, out Pet);
        }

        public bool TryGetBot(int BotId, out RoomUser Bot)
        {
            return this._bots.TryGetValue(BotId, out Bot);
        }

        public void UpdateUserStatus(RoomUser User, bool cyclegameitems)
        {
            if (User == null)
                return;

            if (User._statusses.ContainsKey("lay") || User._statusses.ContainsKey("sit") || User._statusses.ContainsKey("sign"))
            {
                if (User._statusses.ContainsKey("lay"))
                    User.RemoveStatus("lay");
                if (User._statusses.ContainsKey("sit"))
                    User.RemoveStatus("sit");
                if (User._statusses.ContainsKey("sign"))
                    User.RemoveStatus("sign");
                User.UpdateNeeded = true;
            }

            List<Item> roomItemForSquare = this.room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).OrderBy(p => p.GetZ).ToList();

            double newZ = !User.RidingHorse || User.IsPet ? this.room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) : this.room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) + 1.0;
            if (newZ != User.Z)
            {
                User.Z = newZ;
                User.UpdateNeeded = true;
            }

            foreach (Item roomItem in roomItemForSquare)
            {
                if (cyclegameitems)
                {
                    roomItem.UserWalksOnFurni(User, roomItem);

                    if (roomItem.Fx != 0 && !User.IsBot)
                        User.ApplyEffect(roomItem.Fx);
                }

                if (roomItem.GetBaseItem().IsSeat)
                {
                    if (!User._statusses.ContainsKey("sit"))
                    {
                        User.SetStatus("sit", TextHandling.GetString(roomItem.Height));
                        User.IsSit = true;
                    }
                    User.Z = roomItem.GetZ;
                    User.RotHead = roomItem.Rotation;
                    User.RotBody = roomItem.Rotation;
                    User.UpdateNeeded = true;
                }

                switch (roomItem.GetBaseItem().InteractionType)
                {
                    case InteractionType.bed:
                        if (!User._statusses.ContainsKey("lay"))
                        {
                            User.SetStatus("lay", TextHandling.GetString(roomItem.Height) + " null");
                            User.IsLay = true;
                        }
                        User.Z = roomItem.GetZ;
                        User.RotHead = roomItem.Rotation;
                        User.RotBody = roomItem.Rotation;
                        User.UpdateNeeded = true;
                        break;
                    case InteractionType.pressurepad:
                    case InteractionType.TRAMPOLINE:
                    case InteractionType.TREADMILL:
                    case InteractionType.CROSSTRAINER:
                        roomItem.ExtraData = "1";
                        roomItem.UpdateState(false, true);
                        break;
                    case InteractionType.GUILD_GATE:
                        roomItem.ExtraData = "1;" + roomItem.GroupId;
                        roomItem.UpdateState(false, true);
                        break;
                    case InteractionType.ARROW:
                        if (!cyclegameitems || User.IsBot)
                            break;
                        if (roomItem.InteractingUser != 0)
                            break;
                        User.CanWalk = true;
                        roomItem.InteractingUser = User.GetClient().GetHabbo().Id;
                        roomItem.ReqUpdate(2);
                        break;
                    case InteractionType.banzaigateblue:
                    case InteractionType.banzaigatered:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.banzaigategreen:
                        if (cyclegameitems && !User.IsBot)
                        {
                            int EffectId = ((int)roomItem.team + 32);
                            TeamManager managerForBanzai = this.room.GetTeamManager();
                            if (User.team != roomItem.team)
                            {
                                if (User.team != Team.none)
                                {
                                    managerForBanzai.OnUserLeave(User);
                                }
                                User.team = roomItem.team;
                                managerForBanzai.AddUser(User);
                                this.room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect != EffectId)
                                    User.ApplyEffect(EffectId);
                            }
                            else
                            {
                                managerForBanzai.OnUserLeave(User);
                                this.room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect == EffectId)
                                    User.ApplyEffect(0);
                                User.team = Team.none;
                                continue;
                            }
                        }
                        break;
                    case InteractionType.banzaiblo:
                        if (cyclegameitems && User.team != Team.none && !User.IsBot)
                        {
                            this.room.GetGameItemHandler().OnWalkableBanzaiBlo(User, roomItem);
                        }
                        break;
                    case InteractionType.banzaiblob:
                        if (cyclegameitems && User.team != Team.none && !User.IsBot)
                        {
                            this.room.GetGameItemHandler().OnWalkableBanzaiBlob(User, roomItem);
                        }
                        break;
                    case InteractionType.banzaitele:
                        if (cyclegameitems)
                            this.room.GetGameItemHandler().onTeleportRoomUserEnter(User, roomItem);
                        break;
                    case InteractionType.freezeyellowgate:
                    case InteractionType.freezeredgate:
                    case InteractionType.freezegreengate:
                    case InteractionType.freezebluegate:
                        if (cyclegameitems && !User.IsBot)
                        {
                            int EffectId = ((int)roomItem.team + 39);
                            TeamManager managerForFreeze = this.room.GetTeamManager();
                            if (User.team != roomItem.team)
                            {
                                    if (User.team != Team.none)
                                    {
                                        managerForFreeze.OnUserLeave(User);
                                    }
                                    User.team = roomItem.team;
                                    managerForFreeze.AddUser(User);
                                    this.room.GetGameManager().UpdateGatesTeamCounts();
                                    if (User.CurrentEffect != EffectId)
                                        User.ApplyEffect(EffectId);
                            }
                            else
                            {
                                managerForFreeze.OnUserLeave(User);
                                this.room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect == EffectId)
                                    User.ApplyEffect(0);
                                User.team = Team.none;
                            }
                        }
                            break;
                    case InteractionType.fbgate:
                        if (cyclegameitems || string.IsNullOrEmpty(roomItem.ExtraData) || !roomItem.ExtraData.Contains(',') || User == null || User.IsBot || User.transformation || User.IsSpectator)
                            break;

                        if (User.GetClient().GetHabbo().LastMovFGate && User.GetClient().GetHabbo().BackupGender == User.GetClient().GetHabbo().Gender)
                        {
                            User.GetClient().GetHabbo().LastMovFGate = false;
                            User.GetClient().GetHabbo().Look = User.GetClient().GetHabbo().BackupLook;
                        }
                        else
                        {
                            // mini Fix
                            string _gateLook = ((User.GetClient().GetHabbo().Gender.ToUpper() == "M") ? roomItem.ExtraData.Split(',')[0] : roomItem.ExtraData.Split(',')[1]);
                            if (_gateLook == "")
                                break;
                            string gateLook = "";
                            foreach (string part in _gateLook.Split('.'))
                            {
                                if (part.StartsWith("hd"))
                                    continue;
                                gateLook += part + ".";
                            }
                            gateLook = gateLook.Substring(0, gateLook.Length - 1);

                            // Generating New Look.
                            string[] Parts = User.GetClient().GetHabbo().Look.Split('.');
                            string NewLook = "";
                            foreach (string Part in Parts)
                            {
                                if (/*Part.StartsWith("hd") || */Part.StartsWith("sh") || Part.StartsWith("cp") || Part.StartsWith("cc") || Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("ca") || Part.StartsWith("wa"))
                                    continue;
                                NewLook += Part + ".";
                            }
                            NewLook += gateLook;

                            User.GetClient().GetHabbo().BackupLook = User.GetClient().GetHabbo().Look;
                            User.GetClient().GetHabbo().BackupGender = User.GetClient().GetHabbo().Gender;
                            User.GetClient().GetHabbo().Look = NewLook;
                            User.GetClient().GetHabbo().LastMovFGate = true;
                        }

                        User.GetClient().SendPacket(new UserChangeComposer(User, true));

                        if (User.GetClient().GetHabbo().InRoom)
                        {
                            this.room.SendPacket(new UserChangeComposer(User, false));
                        }
                        break;
                    case InteractionType.freezetileblock:
                        if (!cyclegameitems)
                            break;
                        this.room.GetFreeze().OnWalkFreezeBlock(roomItem, User);
                        break;
                    default:
                        break;
                }
            }
            if (cyclegameitems)
            {
                this.room.GetBanzai().HandleBanzaiTiles(User.Coordinate, User.team, User);
            }

            if (User.IsSit || User.IsLay)
            {
                if (User.IsSit)
                {
                    if (!User._statusses.ContainsKey("sit"))
                    {
                        if (User.transformation)
                            User.SetStatus("sit", "");
                        else
                            User.SetStatus("sit", "0.5");

                        User.UpdateNeeded = true;
                    }

                }
                else if (User.IsLay)
                {

                    if (!User._statusses.ContainsKey("lay"))
                    {
                        if (User.transformation)
                            User.SetStatus("lay", "");
                        else
                            User.SetStatus("lay", "0.7");

                        User.UpdateNeeded = true;
                    }

                }
            }
        }

        public void OnCycle(ref int idleCount)
        {
            int userCounter = 0;

            List<RoomUser> ToRemove = new List<RoomUser>();

            foreach (RoomUser User in this.GetUserList().ToList())
            {
                if (!this.isValid(User))
                {
                    if (User.GetClient() != null && User.GetClient().GetHabbo() != null)
                    {
                        this.RemoveUserFromRoom(User.GetClient(), false, false);
                    }
                    else
                        this.RemoveRoomUser(User);
                }

                if (User.mDispose)
                    continue;

                if (User.RidingHorse && User.IsPet)
                    continue;

                if (this.room.RpRoom)
                {
                    RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.room.RoomData.OwnerId);
                    if (RPManager != null)
                    {
                        if (User.IsBot)
                        {
                            if (User.BotData.RoleBot != null)
                                User.BotData.RoleBot.OnCycle(User, this.room);
                        }
                        else
                        {
                            RolePlayer Rp = User.Roleplayer;
                            if (Rp != null)
                                Rp.OnCycle(User, RPManager);
                        }
                    }
                }

                User.IdleTime++;

                if (!User.IsAsleep && User.IdleTime >= 600 && !User.IsBot)
                {
                    User.IsAsleep = true;
                    this.room.SendPacket(new SleepComposer(User, true));
                }

                if (User.NeedsAutokick && !ToRemove.Contains(User))
                {
                    ToRemove.Add(User);
                    continue;
                }

                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    User.CarryTimer--;
                    if (User.CarryTimer <= 0)
                        User.CarryItem(0);
                }

                if (User.UserTimer > 0)
                    User.UserTimer--;

                if (User.FreezeEndCounter > 0)
                {
                    User.FreezeEndCounter--;
                    if (User.FreezeEndCounter <= 0)
                        User.Freeze = false;
                }

                if (User.TimerResetEffect > 0)
                {
                    User.TimerResetEffect--;
                    if (User.TimerResetEffect <= 0)
                    {
                        User.ApplyEffect(User.CurrentEffect, true);
                    }
                }

                if (this.room.GotFreeze())
                    this.room.GetFreeze().CycleUser(User);

                if (User.SetStep)
                {
                    if (SetStepForUser(User))
                        continue;

                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        if (SetStepForUser(roomUserByVirtualId))
                            continue;
                    }
                }
                else
                {
                    User.AllowMoveRoller = true;
                    User.AllowBall = true;
                    User.MoveWithBall = false;
                }

                if (User.IsWalking && !User.Freezed && !User.Freeze && !(this.room.FreezeRoom && (User.GetClient() != null && User.GetClient().GetHabbo().Rank < 6)))
                {
                    CalculatePath(User);

                    User.UpdateNeeded = true;
                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        roomUserByVirtualId.UpdateNeeded = true;
                    }
                }
                else if (User._statusses.ContainsKey("mv"))
                {
                    User.RemoveStatus("mv");
                    User.IsWalking = false;
                    User.UpdateNeeded = true;

                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        roomUserByVirtualId.RemoveStatus("mv");
                        roomUserByVirtualId.IsWalking = false;
                        roomUserByVirtualId.UpdateNeeded = true;
                    }
                }

                if (User.IsBot && User.BotAI != null)
                    User.BotAI.OnTimerTick();
                else if (!User.IsSpectator)
                    userCounter++;
                //if (!updated)
                //this.UpdateUserEffect(User, User.X, User.Y);
            }
            if (userCounter == 0)
                idleCount++;

            foreach (RoomUser user in ToRemove)
            {
                GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(user.HabboId);
                if (clientByUserId != null)
                {
                    this.RemoveUserFromRoom(clientByUserId, true, false);
                }
                else
                    this.RemoveRoomUser(user);
            }
            ToRemove.Clear();

            this.UpdateUserCount(userCounter);
        }

        private void CalculatePath(RoomUser User)
        {
            Gamemap gameMap = this.room.GetGameMap();
            SquarePoint nextStep = DreamPathfinder.GetNextStep(User.X, User.Y, User.GoalX, User.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.mUserOnMap, gameMap.mSquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, User.AllowOverride, gameMap.DiagonalEnabled, this.room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
            if(User.WalkSpeed)
                nextStep = DreamPathfinder.GetNextStep(nextStep.X, nextStep.Y, User.GoalX, User.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.mUserOnMap, gameMap.mSquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, User.AllowOverride, gameMap.DiagonalEnabled, this.room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);


            if (User.breakwalk && User.stopwalking)
            {
                User.stopwalking = false;
                this.UpdateUserStatus(User, false);
                User.RemoveStatus("mv");

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    roomUserByVirtualId.IsWalking = false;
                    this.UpdateUserStatus(roomUserByVirtualId, false);
                    roomUserByVirtualId.RemoveStatus("mv");
                }
            }
            else if (nextStep.X == User.X && nextStep.Y == User.Y || this.room.GetGameItemHandler().CheckGroupGate(User, new Point(nextStep.X, nextStep.Y)))
            {
                User.IsWalking = false;
                this.UpdateUserStatus(User, false);
                User.RemoveStatus("mv");

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    roomUserByVirtualId.IsWalking = false;
                    this.UpdateUserStatus(roomUserByVirtualId, false);
                    roomUserByVirtualId.RemoveStatus("mv");
                }
            }
            else
            {
                HandleSetMovement(nextStep, User);

                if (User.breakwalk && !User.stopwalking)
                    User.stopwalking = true;

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    HandleSetMovement(nextStep, roomUserByVirtualId);
                    roomUserByVirtualId.UpdateNeeded = true;
                }

                if (User.IsSit)
                    User.IsSit = false;
                if (User.IsLay)
                    User.IsLay = false;

                this.room.GetSoccer().OnUserWalk(User, nextStep.X == User.GoalX && nextStep.Y == User.GoalY);
                this.room.GetBanzai().OnUserWalk(User);
            }
        }

        private void HandleSetMovement(SquarePoint nextStep, RoomUser User)
        {
            int nextX = nextStep.X;
            int nextY = nextStep.Y;

            double nextZ = this.room.GetGameMap().SqAbsoluteHeight(nextX, nextY);
            if (User.RidingHorse && !User.IsPet)
                nextZ = nextZ + 1;
            
            User.RemoveStatus("mv");
            User.RemoveStatus("lay");
            User.RemoveStatus("sit");

            User.SetStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

            int newRot;
            if (User.facewalkEnabled)
                newRot = User.RotBody;
            else
                newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.moonwalkEnabled);

            User.RotBody = newRot;
            User.RotHead = newRot;

            User.SetStep = true;
            User.SetX = nextX;
            User.SetY = nextY;
            User.SetZ = nextZ;

            this.room.GetGameMap().AddTakingSquare(nextX, nextY);

            this.UpdateUserEffect(User, User.SetX, User.SetY);
        }

        private bool SetStepForUser(RoomUser User, bool NotUpdate = false)
        {
            this.room.GetGameMap().UpdateUserMovement(User.Coordinate, new Point(User.SetX, User.SetY), User);

            List<Item> coordinatedItems = this.room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).ToList(); //Quitter la dalle


            if (User.IsBot)
            {
                RoomUser BotCollisionUser = this.room.GetGameMap().LookHasUserNearNotBot(User.X, User.Y);
                if (BotCollisionUser != null)
                    this.room.GetWiredHandler().TriggerBotCollision(BotCollisionUser, User.BotData.Name);
            }
            
            User.X = User.SetX;
            User.Y = User.SetY;
            User.Z = User.SetZ;
            
            this.room.CollisionUser(User);

            if (this.room.RpRoom)
            {
                RolePlayer Rp = User.Roleplayer;
                if (Rp != null && !Rp.Dead)
                {
                    ItemTemp ItemTmp = this.room.GetRoomItemHandler().GetFirstTempDrop(User.X, User.Y);
                    if (ItemTmp != null && ItemTmp.InteractionType == InteractionTypeTemp.MONEY)
                    {
                        Rp.Money += ItemTmp.Value;
                        Rp.SendUpdate();
                        if (User.GetClient() != null)
                            User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.pickdollard", User.GetClient().Langue), ItemTmp.Value));

                        User.OnChat("*Récupère un objet au sol*");
                        this.room.GetRoomItemHandler().RemoveTempItem(ItemTmp.Id);
                    }
                    else if (ItemTmp != null && ItemTmp.InteractionType == InteractionTypeTemp.RPITEM)
                    {
                        RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemTmp.Value);
                        if (RpItem != null)
                        {
                            if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
                            {
                                if (User.GetClient() != null)
                                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.itemown", User.GetClient().Langue));
                            }
                            else
                            {
                                Rp.AddInventoryItem(RpItem.Id);

                                if (User.GetClient() != null)
                                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.itempick", User.GetClient().Langue));
                            }
                        }
                        User.OnChat("*Récupère un objet au sol*");
                        this.room.GetRoomItemHandler().RemoveTempItem(ItemTmp.Id);
                    }
                }
            }

            foreach (Item roomItem in coordinatedItems)
            {
                roomItem.UserWalksOffFurni(User, roomItem);

                if (roomItem.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                {
                    roomItem.ExtraData = "0;" + roomItem.GroupId;
                    roomItem.UpdateState(false, true);
                }
                else if (roomItem.GetBaseItem().InteractionType == InteractionType.pressurepad
                    || roomItem.GetBaseItem().InteractionType == InteractionType.TRAMPOLINE
                    || roomItem.GetBaseItem().InteractionType == InteractionType.TREADMILL
                    || roomItem.GetBaseItem().InteractionType == InteractionType.CROSSTRAINER)
                {
                    roomItem.ExtraData = "0";
                    roomItem.UpdateState(false, true);
                }
                else if (roomItem.GetBaseItem().InteractionType == InteractionType.football)
                {
                    if (!User.AllowMoveRoller || roomItem.interactionCountHelper > 0 || this.room.OldFoot)
                        continue;

                    switch (User.RotBody)
                    {
                        case 0:
                            roomItem.MovementDir = MovementDirection.down;
                            break;
                        case 1:
                            roomItem.MovementDir = MovementDirection.downleft;
                            break;
                        case 2:
                            roomItem.MovementDir = MovementDirection.left;
                            break;
                        case 3:
                            roomItem.MovementDir = MovementDirection.upleft;
                            break;
                        case 4:
                            roomItem.MovementDir = MovementDirection.up;
                            break;
                        case 5:
                            roomItem.MovementDir = MovementDirection.upright;
                            break;
                        case 6:
                            roomItem.MovementDir = MovementDirection.right;
                            break;
                        case 7:
                            roomItem.MovementDir = MovementDirection.downright;
                            break;
                    }
                    roomItem.interactionCountHelper = 6;
                    roomItem.InteractingUser = User.VirtualId;
                    roomItem.ReqUpdate(1);
                }
            }

            this.UpdateUserStatus(User, true);
            this.room.GetGameMap().RemoveTakingSquare(User.SetX, User.SetY);

            User.SetStep = false;
            User.AllowMoveRoller = false;

            if (User.SetMoveWithBall)
            {
                User.SetMoveWithBall = false;
                User.MoveWithBall = false;
            }
            return false;
        }

        public void Destroy()
        {
            this.room = (Room)null;
            this.usersByUsername.Clear();
            this.usersByUserID.Clear();
            this.OnUserEnter = (RoomEventDelegate)null;
            this._pets.Clear();
            this._bots.Clear();
            this._users.Clear();
            this._usersRank.Clear();
        }
    }
}
