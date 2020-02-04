using Butterfly.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Text;

using Butterfly.Database.Interfaces;
using System.Data;
using Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd;
using Butterfly.Core;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        private readonly Dictionary<string, ChatCommand> commandRegisterInvokeable;
        private readonly Dictionary<int, string> ListCommande;

        private readonly Dictionary<int, IChatCommand> _commands;

        public CommandManager()
        {
            this._commands = new Dictionary<int, IChatCommand>();
            this.commandRegisterInvokeable = new Dictionary<string, ChatCommand>();
            this.ListCommande = new Dictionary<int, string>();
        }

        public void Init()
        {
            this.InitInvokeableRegister();
            this.RegisterCommand();
        }


        public bool Parse(GameClient Session, RoomUser User, Room Room, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(":"))
                return false;

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            if (!commandRegisterInvokeable.TryGetValue(Split[0].ToLower(), out ChatCommand CmdInfo))
                return false;

            if (!_commands.TryGetValue(CmdInfo.commandID, out IChatCommand Cmd))
                return false;

            int AutorisationType = CmdInfo.UserGotAuthorization2(Session, Room.RoomData.Langue);
            switch (AutorisationType)
            {
                case 2:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.premium", Session.Langue));
                    return true;
                case 3:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.accred", Session.Langue));
                    return true;
                case 4:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.owner", Session.Langue));
                    return true;
                case 5:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue", Session.Langue));
                    return true;
            }
            if (!CmdInfo.UserGotAuthorization(Session))
                return false;

            if (CmdInfo.UserGotAuthorizationStaffLog())
                ButterflyEnvironment.GetGame().GetModerationTool().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().CurrentRoomId, string.Empty, Split[0].ToLower(), string.Format("Tchat commande: {0}", string.Join(" ", Split)));


            Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, User, Split);
            return true;
        }

        private void InitInvokeableRegister()
        {
            this.commandRegisterInvokeable.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM system_commands");
                table = dbClient.GetTable();
            }
            if (table == null)
                return;

            foreach (DataRow dataRow in table.Rows)
            {
                int key = (int)dataRow["id"];
                int pRank = (int)dataRow["minrank"];
                string pDescriptionFr = (string)dataRow["description_fr"];
                string pDescriptionEn = (string)dataRow["description_en"];
                string pDescriptionBr = (string)dataRow["description_br"];
                string input = (string)dataRow["input"];
                string[] strArray = input.ToLower().Split(new char[1] { ',' });

                foreach (string command in strArray)
                {
                    if (this.commandRegisterInvokeable.ContainsKey(command))
                        continue;

                    this.commandRegisterInvokeable.Add(command, new ChatCommand(key, strArray[0], pRank, pDescriptionFr, pDescriptionEn, pDescriptionBr));
                }
            }
        }

        public string GetCommandList(GameClient client)
        {
            int rank = client.GetHabbo().Rank;
            if (this.ListCommande.ContainsKey(rank))
                return this.ListCommande[rank];

            List<string> NotDoublons = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (ChatCommand chatCommand in this.commandRegisterInvokeable.Values)
            {
                if (chatCommand.UserGotAuthorization(client) && !NotDoublons.Contains(chatCommand.input))
                {
                    if (client.Langue == Language.ANGLAIS)
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionEn + "\r\r");
                    else if (client.Langue == Language.PORTUGAIS)
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionBr + "\r\r");
                    else
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionFr + "\r\r");

                    NotDoublons.Add(chatCommand.input);
                }
            }
            NotDoublons.Clear();

            ListCommande.Add(rank, (stringBuilder).ToString());
            return (stringBuilder).ToString();
        }

        public void Register(int CommandId, IChatCommand Command)
        {
            this._commands.Add(CommandId, Command);
        }


        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < Params.Length; ++index)
            {
                if (index >= Start)
                {
                    if (index > Start)
                        stringBuilder.Append(" ");
                    stringBuilder.Append(Params[index]);
                }
            }
            return (stringBuilder).ToString();
        }

        public void RegisterCommand()
        {
            this._commands.Clear();

            this.Register(1, new pickall());
            this.Register(2, new setspeed());
            this.Register(3, new unload());
            this.Register(4, new disablediagonal());
            this.Register(5, new Setmax());
            this.Register(6, new overridee());
            this.Register(7, new teleport());
            this.Register(10, new roomalert());
            this.Register(11, new coords());
            this.Register(12, new coins());
            this.Register(14, new handitem());
            this.Register(15, new hotelalert());
            this.Register(16, new freeze());
            this.Register(18, new enable());
            this.Register(19, new roommute());
            this.Register(23, new roombadge());
            this.Register(24, new massbadge());
            this.Register(26, new userinfo());
            this.Register(28, new shutdown());
            this.Register(30, new giveBadge());
            this.Register(31, new invisible());
            this.Register(32, new ban());
            this.Register(33, new disconnect());
            this.Register(34, new superban());
            this.Register(36, new roomkick());
            this.Register(37, new mute());
            this.Register(38, new unmute());
            this.Register(39, new alert());
            this.Register(40, new kick());
            this.Register(41, new commands());
            this.Register(42, new about());
            this.Register(43, new info());
            this.Register(52, new forcerot());
            this.Register(53, new seteffect());
            this.Register(54, new emptyitems());
            this.Register(60, new warp());
            this.Register(61, new deleteMission());
            this.Register(62, new follow());
            this.Register(63, new come());
            this.Register(64, new moonwalk());
            this.Register(65, new push());
            this.Register(66, new pull());
            this.Register(67, new copylook());
            this.Register(69, new sit());
            this.Register(70, new lay());
            this.Register(84, new transf());
            this.Register(85, new transfstop());
            this.Register(86, new kickall());
            this.Register(87, new troc());
            this.Register(88, new textamigo());
            this.Register(89, new Ipban());
            this.Register(90, new giveitem());
            this.Register(91, new roommutepet());
            this.Register(92, new facewalk());
            this.Register(94, new addfilter());
            this.Register(95, new noface());
            this.Register(96, new emptypets());
            this.Register(97, new construit());
            this.Register(98, new construitstop());
            this.Register(100, new spull());
            this.Register(101, new teleportstaff());
            this.Register(102, new trigger());
            this.Register(105, new roomfreeze());
            this.Register(106, new RemoveBadge());
            this.Register(107, new roomenable());
            this.Register(108, new vipprotect());
            this.Register(109, new machineban());
            this.Register(111, new unloadroom());
            this.Register(112, new warpstaff());
            this.Register(115, new eventalert());
            this.Register(116, new control());
            this.Register(117, new say());
            this.Register(118, new setcopylook());
            this.Register(119, new settransf());
            this.Register(120, new settransfstop());
            this.Register(121, new setenable());
            this.Register(122, new givelot());
            this.Register(123, new extrabox());
            this.Register(124, new saybot());
            this.Register(126, new setz());
            this.Register(127, new setzstop());
            this.Register(128, new murmur());
            this.Register(130, new emptybots());
            this.Register(132, new vip());
            this.Register(133, new followme());
            this.Register(134, new disableoblique());
            this.Register(135, new addphoto());
            this.Register(138, new infosuperwired());
            this.Register(140, new transfbot());
            this.Register(141, new settransfbot());
            this.Register(143, new ShowGuide());
            this.Register(144, new janken());
            this.Register(145, new randomlook());
            this.Register(146, new mazo());
            this.Register(148, new loadvideo());
            this.Register(149, new hidewireds());
            this.Register(150, new warpall());
            this.Register(151, new use());
            this.Register(152, new usestop());
            this.Register(153, new youtube());
            this.Register(154, new RoomSell());
            this.Register(155, new RoomBuy());
            this.Register(156, new RoomRemoveSell());
            this.Register(157, new DupliRoom());
            this.Register(158, new Tir());
            this.Register(159, new SuperBot());
            this.Register(160, new OldFoot());
            this.Register(161, new Pyramide());
            this.Register(162, new Cac());
            this.Register(163, new Pan());
            this.Register(165, new Prison());
            this.Register(166, new Refresh());
            this.Register(168, new MaxFloor());
            this.Register(169, new AutoFloor());
            this.Register(170, new emblem());
            this.Register(171, new Givemoney());
            this.Register(172, new ConfigBot());
            this.Register(173, new SpeedWalk());
            this.Register(174, new ChutAll());
            this.Register(175, new Flagme());
            this.Register(176, new IgnoreAll());
            this.Register(177, new PushNotif());
            this.Register(178, new Big());
            this.Register(179, new Little());
            this.Register(180, new LoadRoomItems());
        }
    }
}