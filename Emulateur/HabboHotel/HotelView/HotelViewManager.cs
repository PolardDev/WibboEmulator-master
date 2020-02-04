using System;
using System.Collections.Generic;
using System.Data;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;


namespace Butterfly.HabboHotel.HotelView
{
    public class HotelViewManager
    {
        public List<SmallPromo> HotelViewPromosIndexers = new List<SmallPromo>();

        public HotelViewManager()
        {
            this.InitHotelViewPromo();
        }

        private void InitHotelViewPromo()
        {
            HotelViewPromosIndexers.Clear();
            DataTable dTable;
            using (IQueryAdapter DbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT * from hotelview_promos WHERE hotelview_promos.enabled = '1' ORDER BY hotelview_promos.`index` ASC");
                dTable = DbClient.GetTable();
            }

            foreach (DataRow dRow in dTable.Rows)
            {
                HotelViewPromosIndexers.Add(new SmallPromo(Convert.ToInt32(dRow[0]), (string)dRow[1], (string)dRow[2], (string)dRow[3], Convert.ToInt32(dRow[4]), (string)dRow[5], (string)dRow[6]));
            }
        }

        public void RefreshPromoList()
        {
            this.InitHotelViewPromo();
        }

        public ServerPacket SmallPromoComposer(ServerPacket Message)
        {
            Message.WriteInteger(HotelViewPromosIndexers.Count);
            foreach (SmallPromo promo in HotelViewPromosIndexers)
            {
                promo.Serialize(Message);
            }

            return Message;
        }

        public static SmallPromo load(int index)
        {
            DataRow dRow;
            using (IQueryAdapter DbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT hotelview_promos.`index`,hotelview_promos.header,hotelview_promos.body,hotelview_promos.button,hotelview_promos.in_game_promo,hotelview_promos.special_action," +
                    "hotelview_promos.image,hotelview_promos.enabled FROM hotelview_promos WHERE hotelview_promos.`index` = @x LIMIT 1");
                DbClient.AddParameter("x", index);
                dRow = DbClient.GetRow();
            }

            SmallPromo newPromo = new SmallPromo(index, (string)dRow[1], (string)dRow[2], (string)dRow[3], Convert.ToInt32(dRow[4]), (string)dRow[5], (string)dRow[6]);
            return newPromo;
        }
    }
}
