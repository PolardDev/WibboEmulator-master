using Butterfly.HabboHotel.Users;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class GroupMemberUpdatedComposer : ServerPacket
    {
        public GroupMemberUpdatedComposer(int GroupId, Habbo Habbo, int Type)
            : base(ServerPacketHeader.GroupMemberUpdatedMessageComposer)
        {
            WriteInteger(GroupId);//GroupId
            WriteInteger(Type);//Type?
            {
                WriteInteger(Habbo.Id);//UserId
                WriteString(Habbo.Username);
                WriteString(Habbo.Look);
                WriteString(string.Empty);
            }
        }
    }
}
