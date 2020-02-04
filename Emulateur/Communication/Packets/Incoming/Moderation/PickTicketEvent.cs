using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class PickTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            Packet.PopInt();
            ButterflyEnvironment.GetGame().GetModerationTool().PickTicket(Session, Packet.PopInt());

        }
    }
}
