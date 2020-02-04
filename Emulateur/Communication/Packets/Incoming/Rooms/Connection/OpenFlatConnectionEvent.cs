using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class OpenFlatConnectionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();
            
            Session.GetHabbo().PrepareRoom(RoomId, Password);
        }
    }
}