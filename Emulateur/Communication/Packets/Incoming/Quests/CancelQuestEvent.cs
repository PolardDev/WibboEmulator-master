using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class CancelQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ButterflyEnvironment.GetGame().GetQuestManager().CancelQuest(Session, Packet);
        }
    }
}
