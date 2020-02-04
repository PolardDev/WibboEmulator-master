using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    class ObjectAddComposer : ServerPacket
    {
        public ObjectAddComposer(Item Item, string Username, int UserId)
            : base(ServerPacketHeader.ObjectAddMessageComposer)
        {
            WriteInteger(Item.Id);
            WriteInteger(Item.GetBaseItem().SpriteId);
            WriteInteger(Item.GetX);
            WriteInteger(Item.GetY);
            WriteInteger(Item.Rotation);
            WriteString(string.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
            WriteString(string.Empty);

            if (Item.LimitedNo > 0)
            {
                WriteInteger(1);
                WriteInteger(256);
                WriteString(Item.ExtraData);
                WriteInteger(Item.LimitedNo);
                WriteInteger(Item.LimitedTot);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            WriteInteger(-1); // to-do: check
            WriteInteger(1);
            WriteInteger(UserId);
            WriteString(Username);
        }

        public ObjectAddComposer(ItemTemp Item)
            : base(ServerPacketHeader.ObjectAddMessageComposer)
        {
            WriteInteger(Item.Id);
            WriteInteger(Item.SpriteId); //ScriptId
            WriteInteger(Item.X);
            WriteInteger(Item.Y);
            WriteInteger(2);
            WriteString(string.Format("{0:0.00}", TextHandling.GetString(Item.Z)));
            WriteString("");

            if (Item.InteractionType == InteractionTypeTemp.RPITEM)
            {
                WriteInteger(0);
                WriteInteger(1);

                WriteInteger(5);

                WriteString("state");
                WriteString("0");
                WriteString("imageUrl");
                WriteString("https://swf.wibbo.me/items/" + Item.ExtraData + ".png");
                WriteString("offsetX");
                WriteString("-20");
                WriteString("offsetY");
                WriteString("10");
                WriteString("offsetZ");
                WriteString("10002");
            }
            else
            {
                WriteInteger(1);
                WriteInteger(0);
                WriteString(Item.ExtraData); //ExtraData
            }


            WriteInteger(-1); // to-do: check
            WriteInteger(1);
            WriteInteger(Item.VirtualUserId);
            WriteString("");
        }
    }
}
