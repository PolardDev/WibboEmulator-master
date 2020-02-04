using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly.HabboHotel.HotelView
{
    public class SmallPromo
    {
        readonly int Index;
        readonly string Header;
        readonly string Body;
        readonly string Button;
        readonly int inGamePromo;
        readonly string SpecialAction;
        readonly string Image;

        public SmallPromo(int index, string header, string body, string button, int inGame, string specialAction, string image)
        {
            this.Index = index;
            this.Header = header;
            this.Body = body;
            this.Button = button;
            this.inGamePromo = inGame;
            this.SpecialAction = specialAction;
            this.Image = image;
        }

        public ServerPacket Serialize(ServerPacket Composer)
        {
            Composer.WriteInteger(Index);
            Composer.WriteString(Header);
            Composer.WriteString(Body);
            Composer.WriteString(Button);
            Composer.WriteInteger(inGamePromo);
            Composer.WriteString(SpecialAction);
            Composer.WriteString(Image);
            return Composer;
        }

    }
}
