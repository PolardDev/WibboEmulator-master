using Butterfly.HabboHotel.GameClients;
            {
                return;
            }
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                return;
            }

                if (clientByUsername.GetHabbo().Rank > 5 && Session.GetHabbo().Rank < 13)
                {
                    ButterflyEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", (double)788922000, "Votre compte � �t� banni par s�curit�", false, false);
                }