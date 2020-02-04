namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class FloorPlanSendDoorMessageComposer : ServerPacket
    {
        public FloorPlanSendDoorMessageComposer()
            : base(ServerPacketHeader.FloorPlanSendDoorMessageComposer)
        {
			
        }
    }
}
