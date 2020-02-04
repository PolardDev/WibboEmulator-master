using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RefreshCampaignEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            try
            {
                String parseCampaings = Packet.PopString();
                if (parseCampaings.Contains("gamesmaker"))
                    return;

                String campaingName = "";
                String[] parser = parseCampaings.Split(';');

                for (int i = 0; i < parser.Length; i++)
                {
                    if (String.IsNullOrEmpty(parser[i]) || parser[i].EndsWith(","))
                        continue;

                    String[] data = parser[i].Split(',');
                    campaingName = data[1];
                }
                Session.SendPacket(new CampaignComposer(parseCampaings, campaingName));
            }
            catch { }
        }
    }
}
