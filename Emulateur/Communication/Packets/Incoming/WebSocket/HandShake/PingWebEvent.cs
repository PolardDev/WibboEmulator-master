using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    class PingWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new PongComposer());
        }
    }
}
