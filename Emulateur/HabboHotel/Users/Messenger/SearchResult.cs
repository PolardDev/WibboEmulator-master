// Type: Butterfly.HabboHotel.Users.Messenger.SearchResult




using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly.HabboHotel.Users.Messenger
{
    public struct SearchResult
  {
    public int userID;
    public string username;
    public string look;

    public SearchResult(int userID, string username, string look)
    {
      this.userID = userID;
      this.username = username;
      this.look = look;
    }

    public void Searialize(ServerPacket reply)
    {
      reply.WriteInteger(this.userID);
      reply.WriteString(this.username);
      reply.WriteString(""); //motto
      bool b = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.userID) != null;
      reply.WriteBoolean(b);
      reply.WriteBoolean(false);
      reply.WriteString(string.Empty);
      reply.WriteInteger(0);
      reply.WriteString(this.look);
      reply.WriteString(""); //last_online
    }
  }
}
