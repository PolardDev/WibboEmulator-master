using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms;using System;

namespace Butterfly.Communication.Packets.Incoming.Structure{    class GetGuestRoomEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            int roomID = Packet.PopInt();

            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
                return;

            Boolean isLoading = Packet.PopInt() == 1;
            Boolean checkEntry = Packet.PopInt() == 1;

            Session.SendPacket(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));        }    }}