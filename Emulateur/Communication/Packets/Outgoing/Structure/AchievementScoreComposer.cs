namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class AchievementScoreComposer : ServerPacket
    {
        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.AchievementScoreMessageComposer)
        {
            WriteInteger(achScore);
        }
    }
}
