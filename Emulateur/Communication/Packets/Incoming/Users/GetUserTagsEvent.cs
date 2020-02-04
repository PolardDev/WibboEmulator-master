using Butterfly.HabboHotel.GameClients;
using Butterfly.Communication.Packets.Outgoing.Structure;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetUserTagsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();

            Session.SendPacket(new UserTagsComposer(UserId));
        }
    }
}
