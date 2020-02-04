using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.HotelView;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            HotelViewManager currentView = ButterflyEnvironment.GetGame().GetHotelView();            if (Session == null || Session.GetHabbo() == null)                return;            if (!(currentView.HotelViewPromosIndexers.Count > 0))                return;            else            {                ServerPacket Message = currentView.SmallPromoComposer(new ServerPacket(ServerPacketHeader.PromoArticlesMessageComposer));                Session.SendPacket(Message);            }
        }
    }
}
