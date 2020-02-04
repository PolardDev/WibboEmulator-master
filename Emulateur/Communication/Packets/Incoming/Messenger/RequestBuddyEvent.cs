using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RequestBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null || !Session.GetHabbo().GetMessenger().RequestBuddy(Packet.PopString()))
                return;
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_FRIEND, 0);

        }
    }
}
