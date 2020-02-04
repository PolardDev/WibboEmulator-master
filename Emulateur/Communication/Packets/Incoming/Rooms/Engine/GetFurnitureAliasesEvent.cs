using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetFurnitureAliasesMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.FurnitureAliasesMessageComposer);
            Response.WriteInteger(0);
            Session.SendPacket(Response);
        }
    }
}