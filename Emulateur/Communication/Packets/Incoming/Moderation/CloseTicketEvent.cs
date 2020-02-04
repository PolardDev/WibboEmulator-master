using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class CloseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
   if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            int Result = Packet.PopInt();
            Packet.PopInt();
            ButterflyEnvironment.GetGame().GetModerationTool().CloseTicket(Session, Packet.PopInt(), Result);

        }
    }
}
