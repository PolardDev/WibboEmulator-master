using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.Database.Interfaces;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Conditions
{
    public class HasUserInGroup : IWiredCondition, IWired
  {
    private Item item;
    private bool isDisposed;

    public HasUserInGroup(Item item)
    {
      this.item = item;
      this.isDisposed = false;
    }

    public bool AllowsExecution(RoomUser user, Item TriggerItem)
    {
        if (user == null || user.IsBot || user.GetClient() == null || user.GetClient().GetHabbo() == null)
            return false;
        if (item.GetRoom().RoomData.Group == null)
            return false;
        if (!user.GetClient().GetHabbo().MyGroups.Contains(item.GetRoom().RoomData.Group.Id))
            return false;
      return true;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, string.Empty, false, null);
    }

    public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
    {

    }

    public void OnTrigger(GameClient Session, int SpriteId)
    {
        ServerPacket Messagegroup = new ServerPacket(ServerPacketHeader.WiredConditionConfigMessageComposer);
        Messagegroup.WriteBoolean(false);
        Messagegroup.WriteInteger(5);
        Messagegroup.WriteInteger(0);
        Messagegroup.WriteInteger(SpriteId);
        Messagegroup.WriteInteger(this.item.Id);
        Messagegroup.WriteString("");
        Messagegroup.WriteInteger(0);
        Messagegroup.WriteInteger(0);
        Messagegroup.WriteInteger(10);

        Messagegroup.WriteInteger(0);
        Messagegroup.WriteInteger(0);
        Messagegroup.WriteInteger(0);
        Session.SendPacket(Messagegroup);
    }

    public void DeleteFromDatabase(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
    }

    public void Dispose()
    {
      this.isDisposed = true;
      this.item = (Item) null;
    }

    public bool Disposed()
    {
      return this.isDisposed;
    }
  }
}
