using Butterfly.HabboHotel.Achievements;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class CameraURLComposer : ServerPacket
    {
        public CameraURLComposer(string Url)
            : base(ServerPacketHeader.CameraURLComposer)
        {
            WriteString(Url);
        }
    }
}