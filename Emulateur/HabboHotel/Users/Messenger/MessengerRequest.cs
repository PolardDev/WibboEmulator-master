// Type: Butterfly.HabboHotel.Users.Messenger.MessengerRequest




using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly.HabboHotel.Users.Messenger
{
    public class MessengerRequest
  {
    private readonly int ToUser;
    private readonly int FromUser;
    private readonly string mUsername;

    public int To
    {
      get
      {
        return this.ToUser;
      }
    }

    public int From
    {
      get
      {
        return this.FromUser;
      }
    }

    public MessengerRequest(int ToUser, int FromUser, string pUsername)
    {
      this.ToUser = ToUser;
      this.FromUser = FromUser;
      this.mUsername = pUsername;
    }

    public void Serialize(ServerPacket Request)
    {
      Request.WriteInteger(this.FromUser);
      Request.WriteString(this.mUsername);
      Request.WriteString("");
    }
  }
}
