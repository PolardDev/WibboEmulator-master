using Butterfly.HabboHotel.GameClients;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class SubmitNewTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (ButterflyEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
                return;
            string Message = StringCharFilter.Escape(Packet.PopString());
            int TicketType = Packet.PopInt();
            int ReporterId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int RepporteurId = Packet.PopInt();

            ButterflyEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, TicketType, ReporterId, Message);
            ButterflyEnvironment.GetGame().GetModerationTool().ApplySanction(Session, ReporterId);
        }
    }
}
