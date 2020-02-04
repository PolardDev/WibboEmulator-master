// Type: Butterfly.HabboHotel.Quests.Composer.QuestStartedComposer




using Butterfly.HabboHotel.GameClients;
using Butterfly.Communication.Packets.Outgoing;


namespace Butterfly.HabboHotel.Quests.Composer
{
    public class QuestStartedComposer
  {
    public static ServerPacket Compose(GameClient Session, Quest Quest)
    {
      ServerPacket Message = new ServerPacket(ServerPacketHeader.QuestStartedMessageComposer);
      QuestListComposer.SerializeQuest(Message, Session, Quest, Quest.Category);
      return Message;
    }
  }
}
