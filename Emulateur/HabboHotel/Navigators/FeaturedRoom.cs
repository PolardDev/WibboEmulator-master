using Butterfly.Core;

namespace Butterfly.HabboHotel.Navigators
{
    public class FeaturedRoom
    {
        public int RoomId { get; set; }
        public string Image { get; set; }
        public Language Langue { get; }

        public FeaturedRoom(int RoomId, string Image, Language Langue)
        {
            this.RoomId = RoomId;
            this.Image = Image;
            this.Langue = Langue;
        }
    }
}