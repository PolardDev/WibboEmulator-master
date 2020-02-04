using Butterfly.HabboHotel.Roleplay;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    class BotChooseComposer : ServerPacket
    {
        public BotChooseComposer(List<string[]> ChooseList)
          : base(23)
        {
            WriteInteger(ChooseList.Count);

            foreach (string[] Choose in ChooseList)
            {
                WriteString(Choose[0]);
                WriteString(Choose[1]);
                WriteString(Choose[2]);
                WriteString(Choose[3]);
            }
        }
    }
}
