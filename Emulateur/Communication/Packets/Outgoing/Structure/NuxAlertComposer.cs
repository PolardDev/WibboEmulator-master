namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class NuxAlertComposer : ServerPacket
    {
        public NuxAlertComposer(string Message)
            : base(ServerPacketHeader.NuxAlertMessageComposer)
        {
            WriteString(Message);
        }

    }
}
