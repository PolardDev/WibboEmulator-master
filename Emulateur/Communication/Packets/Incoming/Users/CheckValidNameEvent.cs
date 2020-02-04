using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class CheckValidNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null || Session == null)
                return;
            string Name = Packet.PopString();
            ServerPacket Response = new ServerPacket(ServerPacketHeader.NameChangeUpdateMessageComposer);
            switch (this.nameAvailable(Name))
            {
                case -2:
                    Response.WriteInteger(4);
                    Response.WriteString(Name);
                    Response.WriteInteger(0);
                    break;
                case -1:
                    Response.WriteInteger(4);
                    Response.WriteString(Name);
                    Response.WriteInteger(0);
                    break;
                case 0:
                    Response.WriteInteger(5);
                    Response.WriteString(Name);
                    Response.WriteInteger(2);
                    Response.WriteString("--" + Name + "--");
                    Response.WriteString("Xx" + Name + "xX");
                    break;
                default:
                    Response.WriteInteger(0);
                    Response.WriteString(Name);
                    Response.WriteInteger(0);
                    break;
            }
            Session.SendPacket(Response);
        }

        private int nameAvailable(string username)
        {
            username = username.ToLower();
            if (username.Length > 15)
                return -2;
            if (username.Length < 3)
                return -2;
            if (!ButterflyEnvironment.IsValidAlphaNumeric(username))
                return -1;
            return ButterflyEnvironment.usernameExists(username) ? 0 : 1;
        }
    }
}