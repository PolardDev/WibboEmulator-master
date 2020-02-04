using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ItemAddComposer : ServerPacket
    {
        public ItemAddComposer(Item Item, string Username, int UserID)
            : base(ServerPacketHeader.ItemAddMessageComposer)
        {
            WriteString(Item.Id.ToString());
            WriteInteger(Item.GetBaseItem().SpriteId);
            WriteString(Item.wallCoord != null ? Item.wallCoord : string.Empty);

            ItemBehaviourUtility.GenerateWallExtradata(Item, this);

            WriteInteger(-1);
            WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0); // Type New R63 ('use bottom')
            WriteInteger(UserID);
            WriteString(Username);
        }
    }
}
