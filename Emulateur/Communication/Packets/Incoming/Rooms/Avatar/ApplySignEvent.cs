using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ApplySignEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
   Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            roomUserByHabbo.Unidle();
            int num = Packet.PopInt();
            if (roomUserByHabbo._statusses.ContainsKey("sign"))
                roomUserByHabbo.RemoveStatus("sign");
            roomUserByHabbo.SetStatus("sign", Convert.ToString(num));
            roomUserByHabbo.UpdateNeeded = true;

        }
    }
}
