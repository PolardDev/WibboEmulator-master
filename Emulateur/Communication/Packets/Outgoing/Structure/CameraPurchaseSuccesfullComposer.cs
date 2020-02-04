using Butterfly.HabboHotel.Achievements;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class CameraPurchaseSuccesfullComposer : ServerPacket
    {
        public CameraPurchaseSuccesfullComposer()
            : base(ServerPacketHeader.CameraPurchaseSuccesfullComposer)
        {
        }
    }
}