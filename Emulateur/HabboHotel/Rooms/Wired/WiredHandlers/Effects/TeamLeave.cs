using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TeamLeave : IWired, IWiredEffect
  {
    private readonly int itemID;

    public TeamLeave(int itemID)
    {
      this.itemID = itemID;
    }

    public void Handle(RoomUser user, Item TriggerItem)
    {
        if (user != null && !user.IsBot && user.GetClient() != null && user.team != Team.none && user.mRoom != null)
      {
          TeamManager managerForBanzai = user.mRoom.GetTeamManager();
          if (managerForBanzai == null)
              return;
          managerForBanzai.OnUserLeave(user);
          user.mRoom.GetGameManager().UpdateGatesTeamCounts();
          user.ApplyEffect(0);
          user.team = Team.none;
      }
    }

    public void Dispose()
    {

    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
      WiredUtillity.SaveTriggerItem(dbClient,  this.itemID, string.Empty, string.Empty, false, null);
    }

    public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
    {
    }

    public void OnTrigger(GameClient Session, int SpriteId)
    {
        ServerPacket Message = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
        Message.WriteBoolean(false);
        Message.WriteInteger(0);
        Message.WriteInteger(0);
        Message.WriteInteger(SpriteId);
        Message.WriteInteger(this.itemID);
        Message.WriteString("");
        Message.WriteInteger(0);
        Message.WriteInteger(0); //7
        Message.WriteInteger(10);
        Message.WriteInteger(0);
        Message.WriteInteger(0);

        Session.SendPacket(Message);
    }

    public void DeleteFromDatabase(IQueryAdapter dbClient)
    {
      dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" +  this.itemID + "'");
    }
  }
}
