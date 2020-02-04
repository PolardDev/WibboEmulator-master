using Butterfly.HabboHotel.Roleplay;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    class AddInventoryItemRpComposer : ServerPacket
    {
        public AddInventoryItemRpComposer(RPItem Item, int pCount)
          : base(10)
        {
            WriteInteger(Item.Id);
            WriteString(Item.Name);
            WriteString(Item.Desc);
            WriteInteger((int)Item.Category);
            WriteInteger(pCount);
            WriteInteger(Item.UseType);
        }
    }
}
