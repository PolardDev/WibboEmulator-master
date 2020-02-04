// Type: Butterfly.HabboHotel.Users.Messenger.MessengerBuddy




using Butterfly.HabboHotel.GameClients;
using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly.HabboHotel.Users.Messenger
{
    public class MessengerBuddy
    {
        private readonly int UserId;
        private readonly string mUsername;
        private string mLook;
        private int Relation;
        private bool mIsOnline;
        private bool mHideInroom;

        public MessengerBuddy(int UserId, string pUsername, string pLook, int relation)
        {
            this.UserId = UserId;
            this.mUsername = pUsername;
            this.mLook = pLook;
            this.Relation = relation;
        }

        public void UpdateRelation(int Type)
        {
            this.Relation = Type;
        }

        public void UpdateUser()
        {
            GameClient client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null && !client.GetHabbo().GetMessenger().AppearOffline)
            {
                mIsOnline = true;
                mLook = client.GetHabbo().Look;
                mHideInroom = client.GetHabbo().HideInRoom;
            }
            else
            {
                mIsOnline = false;
                mLook = "";
                mHideInroom = true;
            }

        }

        public void Serialize(ServerPacket reply)
        {
            reply.WriteInteger(this.UserId);
            reply.WriteString(this.mUsername);
            reply.WriteInteger(1);
            bool isOnline = this.mIsOnline;
            reply.WriteBoolean(isOnline);

            if (isOnline)
                reply.WriteBoolean(!this.mHideInroom);
            else
                reply.WriteBoolean(false);

            reply.WriteString(isOnline ? this.mLook : "");
            reply.WriteInteger(0);
            reply.WriteString(""); //Motto ?
            reply.WriteString(string.Empty);
            reply.WriteString(string.Empty);
            reply.WriteBoolean(true); // Allows offline messaging
            reply.WriteBoolean(false);
            reply.WriteBoolean(false);
            reply.WriteShort(this.Relation);
        }
    }
}
