using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Pets;
using Butterfly.HabboHotel.RoomBots;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.Communication.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Drawing;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Roleplay;

namespace Butterfly.HabboHotel.Rooms
{
    public class RoomUser : IEquatable<RoomUser>
    {
        public int HabboId;
        public int VirtualId;
        public int RoomId;
        public int IdleTime;
        public int X;
        public int Y;
        public double Z;
        public int CarryItemID;
        public int CarryTimer;
        public int RotHead;
        public int RotBody;
        public bool CanWalk;
        public bool AllowOverride;
        public bool TeleportEnabled;
        public int GoalX;
        public int GoalY;
        public bool SetStep;
        public bool AllowMoveRoller;
        public int SetX;
        public int SetY;
        public double SetZ;
        public int UserId;
        public RoomBot BotData;
        public Pet PetData;
        public BotAI BotAI;
        public ItemEffectType CurrentItemEffect;
        public bool Freezed;
        public int FreezeCounter;
        public Team team;
        public FreezePowerUp banzaiPowerUp;
        public int FreezeLives;
        public bool shieldActive;
        public int shieldCounter;
        public int CountFreezeBall = 1;
        public bool moonwalkEnabled;
        public bool facewalkEnabled;
        public bool RidingHorse;
        public bool IsSit;
        public bool IsLay;
        public int HorseID;
        public bool IsWalking;
        public bool UpdateNeeded;
        public bool IsAsleep;
        public Dictionary<string, string> _statusses;
        public int DanceId;
        public int FloodCount;
        public bool IsSpectator;
        public bool ConstruitMode = false;
        public bool ConstruitZMode = false;
        public double ConstruitHeigth = 1.0;
        private GameClient mClient;
        public Room mRoom;
        public bool Freeze;
        public int FreezeEndCounter;
        public bool transformation;
        public bool transfbot;
        public string transformationrace;

        public bool AllowBall;
        public bool MoveWithBall;
        public bool SetMoveWithBall;
        public bool AllowShoot;

        public string ChatTextColor;

        public string LastMessage;
        public int LastMessageCount;

        public int PartyId;
        public int TimerResetEffect;

        public string LoaderVideoId;

        public int WiredPoints;
        public bool InGame;
        public bool WiredGivelot;

        public int UseCount;
        public bool UseMode;

        public bool breakwalk;
        public bool stopwalking;

        public int LLPartner = 0;

        public int CurrentEffect;

        //RP STATS TEMP
        public List<int> AllowBuyItems;

        public bool mDispose;
        public bool ReverseWalk;
        public bool WalkSpeed;
        internal int UserTimer;

        public Point Coordinate
        {
            get
            {
                return new Point(this.X, this.Y);
            }
        }

        public bool IsPet
        {
            get
            {
                if (this.IsBot)
                    return this.BotData.IsPet;
                else
                    return false;
            }
        }

        public bool IsDancing
        {
            get
            {
                return this.DanceId >= 1;
            }
        }

        public bool NeedsAutokick
        {
            get
            {
                return false;//!this.IsBot && (this.GetClient() == null || this.GetClient().GetHabbo() == null || this.GetClient().GetHabbo().Rank < 2 && this.IdleTime >= 1200);
            }
        }

        public bool IsTrading
        {
            get
            {
                return !this.IsBot && this._statusses.ContainsKey("/trd");
            }
        }

        public Dictionary<string, string> Statusses
        {
            get { return this._statusses; }
            //set { this._statusses = value; }
        }

        public bool IsBot
        {
            get
            {
                return this.BotData != null;
            }
        }

        public RolePlayer Roleplayer
        {
            get
            {
                RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.GetRoom().RoomData.OwnerId);
                if (RPManager == null)
                    return null;

                RolePlayer Rp = RPManager.GetPlayer(this.UserId);
                if (Rp == null)
                    return null;

                return Rp;
            }
        }

        public bool AllowMoveTo { get; set; }

        public RoomUser(int HabboId, int RoomId, int VirtualId, Room room)
        {
            this.Freezed = false;
            this.HabboId = HabboId;
            this.RoomId = RoomId;
            this.VirtualId = VirtualId;
            this.IdleTime = 0;
            this.X = 0;
            this.Y = 0;
            this.Z = 0.0;
            this.RotHead = 0;
            this.RotBody = 0;
            this.UpdateNeeded = true;
            this._statusses = new Dictionary<string, string>();
            this.mRoom = room;
            this.AllowOverride = false;
            this.CanWalk = true;
            this.CurrentItemEffect = ItemEffectType.None;
            this.breakwalk = false;
            this.AllowShoot = false;
            this.AllowBuyItems = new List<int>();
            this.mDispose = false;
            this.AllowMoveTo = true;
        }

        public bool Equals(RoomUser comparedUser)
        {
            if (comparedUser == null)
                return false;

            //if (comparedUser.HabboId > 0 || this.HabboId > 0)
                //return comparedUser.HabboId == this.HabboId;

            return comparedUser.VirtualId == this.VirtualId;
        }

        public string GetUsername()
        {
            if (this.IsBot || this.GetClient() == null || this.GetClient().GetHabbo() == null)
                return string.Empty;
            else
                return this.GetClient().GetHabbo().Username;
        }

        public bool IsOwner()
        {
            if (this.IsBot)
                return false;
            else
                return this.GetUsername() == this.GetRoom().RoomData.OwnerName;
        }

        public void Unidle()
        {
            this.IdleTime = 0;
            if (!this.IsAsleep)
                return;
            this.IsAsleep = false;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.SleepMessageComposer);
            Message.WriteInteger(this.VirtualId);
            Message.WriteBoolean(false);
            this.GetRoom().SendPacket(Message);
        }

        public void Dispose()
        {
            this._statusses.Clear();
            this.mDispose = true;
            this.mRoom = (Room)null;
            this.mClient = (GameClient)null;
        }

        public void SendWhisperChat(string message, bool Info = true)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
            Message.WriteInteger(this.VirtualId);
            Message.WriteString(message);
            Message.WriteInteger(0);
            Message.WriteInteger((Info) ? 34 : 0);
            Message.WriteInteger(0);
            Message.WriteInteger(-1);
            this.GetClient().SendPacket(Message);
        }

        public void OnChatMe(string MessageText, int Color = 0, bool Shout = false)
        {
            int Header = ServerPacketHeader.ChatMessageComposer;
            if (Shout)
                Header = ServerPacketHeader.ShoutMessageComposer;

            ServerPacket Message = new ServerPacket(Header);
            Message.WriteInteger(this.VirtualId);
            Message.WriteString(MessageText);
            Message.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            Message.WriteInteger(Color);
            Message.WriteInteger(0);
            Message.WriteInteger(-1);
            this.GetClient().SendPacket(Message);
        }

        public void OnChat(string MessageText, int Color = 0, bool Shout = false)
        {
            int Header = ServerPacketHeader.ChatMessageComposer;
            if (Shout)
                Header = ServerPacketHeader.ShoutMessageComposer;

            ServerPacket Message = new ServerPacket(Header);
            Message.WriteInteger(this.VirtualId);
            Message.WriteString(MessageText);
            Message.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            Message.WriteInteger(Color);
            Message.WriteInteger(0);
            Message.WriteInteger(-1);
            this.GetRoom().SendPacketOnChat(Message, this, true, (this.team == Team.none && !this.IsBot));
        }

        public static int GetSpeechEmotion(string Message)
        {
            Message = Message.ToLower();
            if (Message.Contains(":)") || Message.Contains(":d") || (Message.Contains("=]") || Message.Contains("=d")) || Message.Contains(":>"))
                return 1;
            if (Message.Contains(">:(") || Message.Contains(":@"))
                return 2;
            if (Message.Contains(":o"))
                return 3;
            return Message.Contains(":(") || Message.Contains("=[") || (Message.Contains(":'(") || Message.Contains("='[")) ? 4 : 0;
        }

        public void MoveTo(Point c)
        {
            this.MoveTo(c.X, c.Y);
        }

        public void MoveTo(int pX, int pY, bool pOverride = false)
        {
            if (!this.GetRoom().GetGameMap().CanWalkState(pX, pY, pOverride) || this.Freeze || !this.AllowMoveTo)
                return;

            this.Unidle();
            if (this.TeleportEnabled && (!this.InGame || this.IsOwner()))
            {
                this.GetRoom().SendPacket(this.GetRoom().GetRoomItemHandler().TeleportUser(this, new Point(pX, pY), 0, this.GetRoom().GetGameMap().SqAbsoluteHeight(pX, pY)));
            }
            else
            {
                this.IsWalking = true;
                this.GoalX = pX;
                this.GoalY = pY;
            }
        }

        public void UnlockWalking()
        {
            this.AllowOverride = false;
            this.CanWalk = true;
        }

        public void SetPosRoller(int pX, int pY, double pZ)
        {
            this.SetX = pX;
            this.SetY = pY;
            this.SetZ = pZ;
            this.SetStep = true;

            this.GetRoom().GetGameMap().AddTakingSquare(pX, pY);

            this.UpdateNeeded = false;
            this.IsWalking = false;
        }

        public void SetPos(int pX, int pY, double pZ)
        {
            this.GetRoom().GetGameMap().UpdateUserMovement(this.Coordinate, new Point(pX, pY), this);
            this.X = pX;
            this.Y = pY;
            this.Z = pZ;

            this.SetX = pX;
            this.SetY = pY;
            this.SetZ = pZ;

            this.GoalX = this.X;
            this.GoalY = this.Y;
            this.SetStep = false;
            this.IsWalking = false;
            this.UpdateNeeded = true;
        }

        public void CarryItem(int Item, bool notTimer = false)
        {
            this.CarryItemID = Item;
            this.CarryTimer = Item <= 0 || notTimer ? 0 : 240;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.CarryObjectMessageComposer);
            Message.WriteInteger(this.VirtualId);
            Message.WriteInteger(Item);
            this.GetRoom().SendPacket(Message);
        }

        public void SetRot(int Rotation, bool HeadOnly, bool IgnoreWalk = false)
        {
            if (this._statusses.ContainsKey("lay") || (this.IsWalking && !IgnoreWalk))
                return;
            int num = this.RotBody - Rotation;
            this.RotHead = this.RotBody;
            if (HeadOnly || this._statusses.ContainsKey("sit"))
            {
                if (this.RotBody == 2 || this.RotBody == 4)
                {
                    if (num > 0)
                        this.RotHead = this.RotBody - 1;
                    else if (num < 0)
                        this.RotHead = this.RotBody + 1;
                }
                else if (this.RotBody == 0 || this.RotBody == 6)
                {
                    if (num > 0)
                        this.RotHead = this.RotBody - 1;
                    else if (num < 0)
                        this.RotHead = this.RotBody + 1;
                }
            }
            else if (num <= -2 || num >= 2)
            {
                this.RotHead = Rotation;
                this.RotBody = Rotation;
            }
            else
                this.RotHead = Rotation;
            this.UpdateNeeded = true;
        }

        public void SetStatus(string Key, string Value)
        {
            if (this._statusses.ContainsKey(Key))
            {
                this._statusses[Key] = Value;
            }
            else
            {
                this._statusses.Add(Key, Value);
            }
        }

        public void RemoveStatus(string Key)
        {
            if (!this._statusses.ContainsKey(Key))
                return;
            this._statusses.Remove(Key);
        }

        public void ApplyEffect(int EffectId, bool DontSave = false)
        {
            if (this.mRoom == null)
                return;
            if (this.RidingHorse && (EffectId != 77 && EffectId != 103))
                return;
            if (this.CurrentEffect == EffectId && !DontSave)
                return;
            if (!DontSave)
                this.CurrentEffect = EffectId;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.AvatarEffectMessageComposer);
            Message.WriteInteger(this.VirtualId);
            Message.WriteInteger(EffectId);
            Message.WriteInteger(2);
            this.mRoom.SendPacket(Message);
        }
        
        public bool SetPetTransformation(string NamePet, int RaceId)
        {
            switch (NamePet.ToLower())
            {
                case "littlebelchonok":
                    {
                        this.transformationrace = "73 0 FFFFFF";
                        break;
                    }
                case "bigbelchonok":
                    {
                        this.transformationrace = "72 0 FFFFFF";
                        break;
                    }
                case "littleboy1":
                    {
                        this.transformationrace = "62 0 FFFFFF";
                        break;
                    }
                case "littleboy2":
                    {
                        this.transformationrace = "63 0 FFFFFF";
                        break;
                    }
                case "littleboy3":
                    {
                        this.transformationrace = "64 0 FFFFFF";
                        break;
                    }
                case "littleboy4":
                    {
                        this.transformationrace = "65 0 FFFFFF";
                        break;
                    }
                case "littleboy5":
                    {
                        this.transformationrace = "66 0 FFFFFF";
                        break;
                    }
                case "littlegirl1":
                    {
                        this.transformationrace = "67 0 FFFFFF";
                        break;
                    }
                case "littlegirl2":
                    {
                        this.transformationrace = "68 0 FFFFFF";
                        break;
                    }
                case "littlegirl3":
                    {
                        this.transformationrace = "69 0 FFFFFF";
                        break;
                    }
                case "littlegirl4":
                    {
                        this.transformationrace = "70 0 FFFFFF";
                        break;
                    }
                case "littlegirl5":
                    {
                        this.transformationrace = "71 0 FFFFFF";
                        break;
                    }
                case "bigboy1":
                    {
                        this.transformationrace = "52 0 FFFFFF";
                        break;
                    }
                case "bigboy2":
                    {
                        this.transformationrace = "53 0 FFFFFF";
                        break;
                    }
                case "bigboy3":
                    {
                        this.transformationrace = "54 0 FFFFFF";
                        break;
                    }
                case "bigboy4":
                    {
                        this.transformationrace = "55 0 FFFFFF";
                        break;
                    }
                case "bigboy5":
                    {
                        this.transformationrace = "56 0 FFFFFF";
                        break;
                    }
                case "biggirl1":
                    {
                        this.transformationrace = "57 0 FFFFFF";
                        break;
                    }
                case "biggirl2":
                    {
                        this.transformationrace = "58 0 FFFFFF";
                        break;
                    }
                case "biggirl3":
                    {
                        this.transformationrace = "59 0 FFFFFF";
                        break;
                    }
                case "biggirl4":
                    {
                        this.transformationrace = "60 0 FFFFFF";
                        break;
                    }
                case "biggirl5":
                    {
                        this.transformationrace = "61 0 FFFFFF";
                        break;
                    }
                case "bigmartial":
                    {
                        this.transformationrace = "51 " + RaceId + " FFFFFF";
                        break;
                    }
                case "littlemartial":
                    {
                        this.transformationrace = "50 " + RaceId + " FFFFFF";
                        break;
                    }
                case "bigzeers":
                    {
                        this.transformationrace = "49 " + RaceId + " FFFFFF";
                        break;
                    }
                case "littlezeers":
                    {
                        this.transformationrace = "48 " + RaceId + " FFFFFF";
                        break;
                    }
                case "bigkodamas":
                    {
                        this.transformationrace = "47 " + RaceId + " FFFFFF";
                        break;
                    }
                case "littlekodamas":
                    {
                        this.transformationrace = "46 " + RaceId + " FFFFFF";
                        break;
                    }
                case "maggie":
                    {
                        this.transformationrace = "45 " + RaceId + " FFFFFF";
                        break;
                    }
                case "vache":
                    {
                        this.transformationrace = "44 " + RaceId + " FFFFFF";
                        break;
                    }
                case "bebe":
                    {
                        this.transformationrace = "27 " + RaceId + " FFFFFF";
                        break;
                    }
                //case "pikachuold":
                //{
                //this.transformationrace = "28 " + RaceId + " FFFFFF";
                //break;
                //}
                case "bebeterrier":
                    {
                        this.transformationrace = "26 " + RaceId + " FFFFFF";
                        break;
                    }
                case "lapinjaune":
                    {
                        this.transformationrace = "24 " + RaceId + " FFFFFF";
                        break;
                    }
                case "singedemon":
                    {
                        this.transformationrace = "23 " + RaceId + " FFFFFF";
                        break;
                    }
                case "pigeonnoir":
                    {
                        this.transformationrace = "22 " + RaceId + " FFFFFF";
                        break;
                    }
                case "pigeonblanc":
                    {
                        this.transformationrace = "21 " + RaceId + " FFFFFF";
                        break;
                    }
                case "lapinrose":
                    {
                        this.transformationrace = "20 " + RaceId + " FFFFFF";
                        break;
                    }
                case "lapinbrun":
                    {
                        this.transformationrace = "19 " + RaceId + " FFFFFF";
                        break;
                    }
                case "lapinnoir":
                    {
                        this.transformationrace = "18 " + RaceId + " FFFFFF";
                        break;
                    }
                case "lapinmonstre":
                    {
                        this.transformationrace = "17 " + RaceId + " FFFFFF";
                        break;
                    }
                case "monster":
                    {
                        this.transformationrace = "15 " + RaceId + " FFFFFF";
                        break;
                    }
                case "monsterplante":
                    {
                        this.transformationrace = "16 " + RaceId + " FFFFFF";
                        break;
                    }
                case "cheval":
                    {
                        this.transformationrace = "13 " + RaceId + " FFFFFF";
                        break;
                    }
                case "singe":
                    {
                        this.transformationrace = "14 " + RaceId + " FFFFFF";
                        break;
                    }
                case "tortue":
                    {
                        this.transformationrace = "9 " + RaceId + " FFFFFF";
                        break;
                    }
                case "dragon":
                    {
                        this.transformationrace = "12 " + RaceId + " FFFFFF";
                        break;
                    }
                case "poussin":
                    {
                        this.transformationrace = "10 " + RaceId + " FFFFFF";
                        break;
                    }
                case "grenouille":
                    {
                        this.transformationrace = "11 " + RaceId + " FFFFFF";
                        break;
                    }
                case "arraigne":
                    {
                        this.transformationrace = "8 " + RaceId + " FFFFFF";
                        break;
                    }
                case "lion":
                    {
                        this.transformationrace = "6 " + RaceId + " FFFFFF";
                        break;
                    }
                case "cochon":
                    {
                        this.transformationrace = "5 " + RaceId + " FFFFFF";
                        break;
                    }
                case "terrier":
                    {
                        this.transformationrace = "3 " + RaceId + " FFFFFF";
                        break;
                    }
                case "ours":
                    {
                        this.transformationrace = "4 " + RaceId + " FFFFFF";
                        break;
                    }
                case "chat":
                    {
                        this.transformationrace = "1 " + RaceId + " FFFFFF";
                        break;
                    }
                case "chien":
                    {
                        this.transformationrace = "0 " + RaceId + " FFFFFF";
                        break;
                    }
                case "crocodile":
                    {
                        this.transformationrace = "2 " + RaceId + " FFFFFF";
                        break;
                    }
                case "rhino":
                    {
                        this.transformationrace = "7 " + RaceId + " FFFFFF";
                        break;
                    }
                case "gnome":
                    {
                        this.transformationrace = "29 " + RaceId + " FFFFFF";
                        break;
                    }
                case "oursons":
                    {
                        this.transformationrace = "25 " + RaceId + " FFFFFF";
                        break;
                    }
                case "bebeelephant":
                    {
                        this.transformationrace = "30 " + RaceId + " FFFFFF";
                        break;
                    }
                case "bebepingouin":
                    {
                        this.transformationrace = "31 " + RaceId + " FFFFFF";
                        break;
                    }
                case "pikachu":
                    {
                        this.transformationrace = "32 " + RaceId + " FFFFFF";
                        break;
                    }
                case "louveteau":
                    {
                        this.transformationrace = "33 " + RaceId + " FFFFFF";
                        break;
                    }
                case "hamster":
                    {
                        this.transformationrace = "34 " + RaceId + " FFFFFF";
                        break;
                    }
                case "oeuf":
                case "monsteregg":
                    {
                        this.transformationrace = "36 " + RaceId + " FFFFFF";
                        break;
                    }
                case "yoshi":
                    {
                        this.transformationrace = "35 " + RaceId + " FFFFFF";
                        break;
                    }
                case "kittenbaby":
                case "chaton":
                    {
                        this.transformationrace = "37 " + RaceId + " FFFFFF";
                        break;
                    }
                case "puppybaby":
                case "chiot":
                    {
                        this.transformationrace = "38 " + RaceId + " FFFFFF";
                        break;
                    }
                case "pigletbaby":
                case "porcelet":
                    {
                        this.transformationrace = "39 " + RaceId + " FFFFFF";
                        break;
                    }
                case "fools":
                case "pierre":
                    {
                        this.transformationrace = "40 " + RaceId + " FFFFFF";
                        break;
                    }
                case "haloompa":
                case "wiloompa":
                    {
                        this.transformationrace = "41 " + RaceId + " FFFFFF";
                        break;
                    }
                case "pterosaur":
                    {
                        this.transformationrace = "42 " + RaceId + " FFFFFF";
                        break;
                    }
                case "velociraptor":
                    {
                        this.transformationrace = "43 " + RaceId + " FFFFFF";
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }
            return true;
        }



        public GameClient GetClient()
        {
            if (this.IsBot)
                return (GameClient)null;
            if (this.mClient == null)
                this.mClient = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.HabboId);
            return this.mClient;
        }

        private Room GetRoom()
        {
            if (this.mRoom == null)
                this.mRoom = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            return this.mRoom;
        }
    }
}
