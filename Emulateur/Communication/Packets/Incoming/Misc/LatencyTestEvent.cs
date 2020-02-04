using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class LatencyTestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.LatencyResponseMessageComposer);
            Response.WriteInteger(Packet.PopInt());
            Session.SendPacket(Response);
        }
    }
}