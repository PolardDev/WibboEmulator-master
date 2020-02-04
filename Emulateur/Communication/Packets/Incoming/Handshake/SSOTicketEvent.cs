using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class SSOTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {            if (Session == null || Session.GetHabbo() != null) //|| Session.RC4Client == null
                return;            Session.TryAuthenticate(Packet.PopString());
        }
    }
}
