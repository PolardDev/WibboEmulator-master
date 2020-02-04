using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class OpenHelpToolEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.OpenHelpToolMessageComposer);
            serverMessage.WriteInteger(0);
            Session.SendPacket(serverMessage);
        }
    }
}
