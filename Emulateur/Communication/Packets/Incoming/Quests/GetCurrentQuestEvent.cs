using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetCurrentQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ButterflyEnvironment.GetGame().GetQuestManager().GetCurrentQuest(Session, Packet);
        }
    }
}