using Butterfly.HabboHotel.GameClients;
                return;

            int userId = Packet.PopInt();

            if (userId == Session.GetHabbo().Id)
                return;

            string Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            if (string.IsNullOrWhiteSpace(Message))
                return;

            Session.GetHabbo().FloodTime = DateTime.Now;
            Session.GetHabbo().FloodCount++;

            if (Session.Antipub(Message, "<MP>"))
                return;

            if (Session.GetHabbo().IgnoreAll)
                return;

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, Message);
        }

    }
}