using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetWardrobeEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new WardrobeComposer(Session));
        }
    }
}