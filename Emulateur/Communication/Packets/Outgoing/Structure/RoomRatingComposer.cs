namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class RoomRatingComposer : ServerPacket
    {
        public RoomRatingComposer(int Score, bool CanVote)
            : base(ServerPacketHeader.RoomRatingMessageComposer)
        {
            WriteInteger(Score);
            WriteBoolean(CanVote);
        }
    }
}
