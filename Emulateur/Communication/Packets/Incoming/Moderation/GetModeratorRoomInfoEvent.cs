using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Support;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
   if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            Session.SendPacket(ModerationManager.SerializeRoomTool(ButterflyEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Packet.PopInt())));

        }
    }
}
