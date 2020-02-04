// Type: Butterfly.HabboHotel.Quests.Composer.QuestAbortedComposer




using Butterfly.Communication.Packets.Outgoing;


namespace Butterfly.HabboHotel.Quests.Composer
{
  public class QuestAbortedComposer
  {
    public static ServerPacket Compose()
    {
      ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.QuestAbortedMessageComposer);
      serverMessage.WriteBoolean(false);
      return serverMessage;
    }
  }
}
