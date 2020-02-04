using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.GameClients;namespace Butterfly.Communication.Packets.Incoming.Structure{    class SaveWardrobeOutfitEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            int num = Packet.PopInt();            string Look = Packet.PopString();            string Gender = Packet.PopString();            if (Gender != "M" && Gender != "F")                return;            Look = ButterflyEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())            {                queryreactor.SetQuery(string.Concat(new object[4]                {                   "SELECT null FROM user_wardrobe WHERE user_id = ",                   Session.GetHabbo().Id,                   " AND slot_id = ",                   num                }));                queryreactor.AddParameter("look", Look);                queryreactor.AddParameter("gender", Gender.ToUpper());                if (queryreactor.GetRow() != null)                {                    queryreactor.SetQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = " + Session.GetHabbo().Id + " AND slot_id = " + num + ";");                    queryreactor.AddParameter("look", Look);                    queryreactor.AddParameter("gender", Gender.ToUpper());                    queryreactor.RunQuery();                }                else                {                    queryreactor.SetQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (" + Session.GetHabbo().Id + "," + num + ",@look,@gender)");                    queryreactor.AddParameter("look", Look);                    queryreactor.AddParameter("gender", Gender.ToUpper());                    queryreactor.RunQuery();                }            }        }    }}