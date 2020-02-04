using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class GetRoomBannedUsersComposer : ServerPacket
    {
        public GetRoomBannedUsersComposer(Room instance)
            : base(ServerPacketHeader.GetRoomBannedUsersMessageComposer)
        {
            WriteInteger(instance.Id);

            WriteInteger(instance.getBans().Count);//Count
            foreach (int Id in instance.getBans().Keys)
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
