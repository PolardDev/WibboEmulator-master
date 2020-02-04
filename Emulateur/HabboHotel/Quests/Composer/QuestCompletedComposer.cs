// Type: Butterfly.HabboHotel.Quests.Composer.QuestCompletedComposer




using Butterfly.HabboHotel.GameClients;
using Butterfly.Communication.Packets.Outgoing;


namespace Butterfly.HabboHotel.Quests.Composer
{
    public class QuestCompletedComposer
  {
    public static ServerPacket Compose(GameClient Session, Quest Quest)
    {
      ServerPacket Message = new ServerPacket(ServerPacketHeader.QuestCompletedMessageComposer);
      QuestListComposer.SerializeQuest(Message, Session, Quest, Quest.Category);
      Message.WriteBoolean(true);
      return Message;
    }
  }
}
