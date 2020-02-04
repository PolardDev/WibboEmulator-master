using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RecomendHelpers : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {            ServerPacket Response = new ServerPacket(ServerPacketHeader.OnGuideSessionDetached);            Session.SendPacket(Response);
        }
    }
}
