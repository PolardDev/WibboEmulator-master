using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using System.Data;
using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class BotTalkToAvatar : IWired, IWiredEffect
    {
        private WiredHandler handler;
        private readonly int itemID;
        private string NomBot;
        private string message;
        private bool IsMurmur;

        public BotTalkToAvatar(string nombot, string message, bool iscrier, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;
            this.NomBot = nombot;
            this.IsMurmur = iscrier;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.NomBot == "" || this.message == "" || user == null || user.GetClient() == null)
                return;
            Room room = handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NomBot);
            if (Bot == null)
                return;

            string TextMessage = this.message;
            if (user != null)
            {
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.handler.GetRoom().RoomData.Name.ToString());
            }

            if (IsMurmur)
            {
                ServerPacket Message = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                Message.WriteInteger(Bot.VirtualId);
                Message.WriteString(TextMessage);
                Message.WriteInteger(0);
                Message.WriteInteger(2);
                Message.WriteInteger(0);
                Message.WriteInteger(-1);
                user.GetClient().SendPacket(Message);
            }
            else
            {
                ServerPacket Message = new ServerPacket(ServerPacketHeader.ChatMessageComposer);
                Message.WriteInteger(Bot.VirtualId);
                Message.WriteString(TextMessage);
                Message.WriteInteger(RoomUser.GetSpeechEmotion(this.message));
                Message.WriteInteger(2);
                Message.WriteInteger(0);
                Message.WriteInteger(-1);
                user.GetClient().SendPacket(Message);
            }
        }

        public void Dispose()
        {
            this.message = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NomBot + '\t' + this.message, this.IsMurmur, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, all_user_triggerable FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;

            this.IsMurmur = (bool)(row["all_user_triggerable"]);

            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
                return;

            string[] SplitData = Data.Split('\t');

            this.NomBot = SplitData[0].ToString();
            this.message = SplitData[1].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.NomBot + '\t' + this.message);
            Message15.WriteInteger(1);
            Message15.WriteInteger(TextHandling.BooleanToInt(this.IsMurmur));
            Message15.WriteInteger(0);
            Message15.WriteInteger(27); //7
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
