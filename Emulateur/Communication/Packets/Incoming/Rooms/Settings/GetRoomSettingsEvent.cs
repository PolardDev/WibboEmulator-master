using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            Session.SendPacket(new RoomSettingsDataComposer(Room.RoomData));
        }
    }
}