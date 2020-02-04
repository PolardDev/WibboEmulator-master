using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class RoomRightsListComposer : ServerPacket
    {
        public RoomRightsListComposer(Room Instance)
            : base(ServerPacketHeader.RoomRightsListMessageComposer)
        {
            WriteInteger(Instance.Id);

            WriteInteger(Instance.UsersWithRights.Count);
            foreach (int Id in Instance.UsersWithRights.ToList())
            {
                Habbo Data = ButterflyEnvironment.GetHabboById(Id);
                if (Data == null)
                {
                    WriteInteger(0);
                    WriteString("Unknown Error");
                }
                else
                {
                    WriteInteger(Data.Id);
                    WriteString(Data.Username);
                }
            }
        }
    }
}
