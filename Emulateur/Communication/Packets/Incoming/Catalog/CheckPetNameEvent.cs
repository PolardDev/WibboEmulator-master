using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class CheckPetNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string str = Packet.PopString();

            ServerPacket Response = new ServerPacket(ServerPacketHeader.CheckPetNameMessageComposer);
            Response.WriteInteger(PetUtility.CheckPetName(str) ? 0 : 2);
            Response.WriteString(str);
            Session.SendPacket(Response);
        }
    }
}