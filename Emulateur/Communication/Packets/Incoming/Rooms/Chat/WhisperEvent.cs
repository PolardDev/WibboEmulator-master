using Butterfly.Communication.Packets.Outgoing;using Butterfly.Communication.Packets.Outgoing.Structure;using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms;using Butterfly.HabboHotel.Rooms.Chat.Styles;using Butterfly.Utilities;using System;using System.Collections.Generic;namespace Butterfly.Communication.Packets.Incoming.Structure{    class WhisperEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (Session == null || Session.GetHabbo() == null)                return;            Room Room = Session.GetHabbo().CurrentRoom;            if (Room == null)                return;                        if (Room.UserIsMuted(Session.GetHabbo().Id))            {                if (!Room.HasMuteExpired(Session.GetHabbo().Id))                    return;                else                    Room.RemoveMute(Session.GetHabbo().Id);            }            string Params = StringCharFilter.Escape(Packet.PopString());            if (string.IsNullOrEmpty(Params) || Params.Length > 100 || !Params.Contains(" "))                return;            string ToUser = Params.Split(new char[1] { ' ' })[0];            if (ToUser.Length + 1 > Params.Length)                return;            string Message = Params.Substring(ToUser.Length + 1);            int Color = Packet.PopInt();

            ChatStyle Style = null;            if (!ButterflyEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Color, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().HasFuse(Style.RequiredRight)))                Color = 0;

            if (Session.Antipub(Message, "<MP>"))
                return;            if (!Session.GetHabbo().HasFuse("word_filter_override"))                Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(ToUser);
            
            if (User == null || User2 == null || User2.GetClient() == null || User2.GetClient().GetHabbo() == null)                return;            if (User.IsSpectator || User2.IsSpectator)                return;

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().spamFloodTime;
            if (timeSpan.TotalSeconds > (double)Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                User.FloodCount = 0;
                Session.GetHabbo().spamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
                User.FloodCount = 0;

            if (timeSpan.TotalSeconds < (double)Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                int i = Session.GetHabbo().spamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.GetHabbo().spamProtectionTime = (Room.RpRoom) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetHabbo().spamProtectionTime = (Room.RpRoom) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                    User.LastMessageCount++;                User.LastMessage = Message;                Session.GetHabbo().spamFloodTime = DateTime.Now;                User.FloodCount++;                if (Message.StartsWith("@red@"))
                    User.ChatTextColor = "@red@";
                if (Message.StartsWith("@cyan@"))
                    User.ChatTextColor = "@cyan@";
                if (Message.StartsWith("@blue@"))
                    User.ChatTextColor = "@blue@";
                if (Message.StartsWith("@green@"))
                    User.ChatTextColor = "@green@";
                if (Message.StartsWith("@purple@"))
                    User.ChatTextColor = "@purple@";
                if (Message.StartsWith("@black@"))
                    User.ChatTextColor = "";

                if (!string.IsNullOrEmpty(User.ChatTextColor))
                    Message = User.ChatTextColor + " " + Message;


                ServerPacket Message1 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                Message1.WriteInteger(User.VirtualId);
                Message1.WriteString(Message);
                Message1.WriteInteger(RoomUser.GetSpeechEmotion(Message));
                Message1.WriteInteger(Color);
                Message1.WriteInteger(0);
                Message1.WriteInteger(-1);
                User.GetClient().SendPacket(Message1);

                User.Unidle();

                if (!User2.IsBot && (User2.UserId != User.UserId && !User2.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id)) && !Session.GetHabbo().IgnoreAll)
                {
                    User2.GetClient().SendPacket(Message1);
                    if (User.GetUsername() != "Jason" && User2.GetUsername() != "Jason")
                    {
                        Session.GetHabbo().GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                        Room.GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                    }
                }                if (User.GetUsername() == "Jason" || User2.GetUsername() == "Jason")                    return;                List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();                if (roomUserByRank.Count <= 0)                    return;                ServerPacket Message2 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);                Message2.WriteInteger(User.VirtualId);                Message2.WriteString(ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);                Message2.WriteInteger(RoomUser.GetSpeechEmotion(Message));                Message2.WriteInteger(Color);                Message2.WriteInteger(0);                Message2.WriteInteger(-1);                foreach (RoomUser roomUser in roomUserByRank)                {                    if (roomUser != null && User2 != null && roomUser.HabboId != User2.HabboId && (roomUser.HabboId != User.HabboId && roomUser.GetClient() != null) && roomUser.GetClient().GetHabbo().ViewMurmur)                        roomUser.GetClient().SendPacket(Message2);                }            }        }    }}