using Butterfly.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Janken
{
    public class JankenManager
    {
        private ConcurrentDictionary<int, Janken> mJankenParty;
        private List<Janken> mRemoveParty;
        private Room mRoom;

        public JankenManager(Room room)
        {
            this.mJankenParty = new ConcurrentDictionary<int, Janken>();
            this.mRemoveParty = new List<Janken>();
            this.mRoom = room;
        }

        public void Duel(RoomUser User, RoomUser DuelUser)
        {
            if (User.PartyId > 0)
            {
                Janken party = GetParty(User.PartyId);
                if (party == null)
                {
                    User.PartyId = 0;
                    return;
                }
                if (party.Started)
                    return;

                this.mJankenParty.TryRemove(User.PartyId, out party);
            }

            if (DuelUser.PartyId > 0)
            {
                Janken party = GetParty(DuelUser.PartyId);
                if (party == null)
                {
                    DuelUser.PartyId = 0;
                    return;
                }

                if (party.UserTwo == User.UserId)
                {
                    party.Started = true;
                    User.PartyId = party.UserOne;

                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.start", User.GetClient().Langue));
                    DuelUser.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.start", DuelUser.GetClient().Langue));
                }
                else
                {
                    User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.notwork", User.GetClient().Langue), DuelUser.GetUsername()));
                }
            }
            else
            {
                User.PartyId = User.UserId;
                mJankenParty.TryAdd(User.PartyId, new Janken(User.UserId, DuelUser.UserId));

                User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.wait", User.GetClient().Langue), DuelUser.GetUsername()));
                DuelUser.SendWhisperChat(User.GetUsername() + " vous défie au JanKen! Utilisez la commande :janken " + User.GetUsername() + " pour accepter le défie");
            }
        }

        public void OnCycle()
        {
            if (this.mJankenParty.Count == 0)
                return;

            foreach (Janken party in this.mJankenParty.Values)
            {
                if (!party.Started)
                    continue;

                party.Timer++;

                if (party.Timer >= 60 || (party.ChoixOne != JankenEnum.None && party.ChoixTwo != JankenEnum.None))
                {
                    if (EndGame(party))
                    {
                        party.Started = false;

                        RoomUser roomuserOne = mRoom.GetRoomUserManager().GetRoomUserByHabboId(party.UserOne);
                        RoomUser roomuserTwo = mRoom.GetRoomUserManager().GetRoomUserByHabboId(party.UserTwo);

                        if (roomuserOne != null)
                            roomuserOne.PartyId = 0;

                        if (roomuserTwo != null)
                            roomuserTwo.PartyId = 0;

                        mRemoveParty.Add(party);
                    }
                }
            }

            if (mRemoveParty.Count > 0)
            {
                foreach (Janken party in mRemoveParty)
                {
                    Janken outparty;
                    mJankenParty.TryRemove(party.UserOne, out outparty);
                }

                mRemoveParty.Clear();
            }
        }

        public bool PlayerStarted(RoomUser User)
        {
            Janken party = GetParty(User.PartyId);
            if (party == null)
                return false;

            if (!party.Started)
                return false;

            if (party.UserOne == User.UserId && party.ChoixOne != JankenEnum.None)
                return false;

            if (party.UserTwo == User.UserId && party.ChoixTwo != JankenEnum.None)
                return false;

            return true;
        }

        public bool PickChoix(RoomUser User, string Message)
        {
            Janken party = GetParty(User.PartyId);

            JankenEnum Choix = JankenEnum.None;
            if (Message.ToLower().StartsWith("p"))
                Choix = JankenEnum.Pierre;
            else if (Message.ToLower().StartsWith("f"))
                Choix = JankenEnum.Feuille;
            else if (Message.ToLower().StartsWith("c"))
                Choix = JankenEnum.Ciseaux;
            else
                return false;

            if (party.UserOne == User.UserId)
                party.ChoixOne = Choix;
            else
                party.ChoixTwo = Choix;
            
            if(User.GetClient() != null)
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.confirmechoice", User.GetClient().Langue) + this.GetSringChoix(Choix, User.GetClient().Langue));

            return true;
        }

        private bool EndGame(Janken party)
        {
            RoomUser roomuserOne = mRoom.GetRoomUserManager().GetRoomUserByHabboId(party.UserOne);
            RoomUser roomuserTwo = mRoom.GetRoomUserManager().GetRoomUserByHabboId(party.UserTwo);
            if (roomuserOne == null && roomuserTwo == null)
            {
                return true;
            }

            if (roomuserOne == null)
            {
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserTwo.GetClient().Langue));
                return true;
            }
            else if (roomuserTwo == null)
            {
                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserOne.GetClient().Langue));
                return true;
            }

            if (party.ChoixOne == JankenEnum.None && party.ChoixTwo == JankenEnum.None)
            {
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.annule", roomuserTwo.GetClient().Langue));
                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.annule", roomuserOne.GetClient().Langue));
                return true;
            }

            if (party.ChoixOne == JankenEnum.None)
            {
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserTwo.GetClient().Langue));
                return true;
            }
            else if (party.ChoixTwo == JankenEnum.None)
            {
                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserOne.GetClient().Langue));
                return true;
            }

            if (party.ChoixOne == party.ChoixTwo) //match null
            {
                party.ChoixOne = JankenEnum.None;
                party.ChoixTwo = JankenEnum.None;

                party.Timer = 0;

                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.nul", roomuserOne.GetClient().Langue));
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.nul", roomuserTwo.GetClient().Langue));

                EnableEffet(roomuserOne, party.ChoixOne);
                EnableEffet(roomuserTwo, party.ChoixTwo);
                return false;
            }

            //ChoixOne qui gagne
            if ((party.ChoixOne == JankenEnum.Ciseaux && party.ChoixTwo == JankenEnum.Feuille) ||
                (party.ChoixOne == JankenEnum.Feuille && party.ChoixTwo == JankenEnum.Pierre) ||
                (party.ChoixOne == JankenEnum.Pierre && party.ChoixTwo == JankenEnum.Ciseaux))
            {
                roomuserOne.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.win", roomuserOne.GetClient().Langue), this.GetSringChoix(party.ChoixOne, roomuserOne.GetClient().Langue), this.GetSringChoix(party.ChoixTwo, roomuserOne.GetClient().Langue), roomuserTwo.GetUsername()));
                roomuserTwo.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.loose", roomuserTwo.GetClient().Langue), this.GetSringChoix(party.ChoixOne, roomuserTwo.GetClient().Langue), this.GetSringChoix(party.ChoixTwo, roomuserTwo.GetClient().Langue), roomuserOne.GetUsername()));

                EnableEffet(roomuserOne, party.ChoixOne);
                EnableEffet(roomuserTwo, party.ChoixTwo);
                return true;
            }

            //ChoixTwo qui gagne
            if ((party.ChoixOne == JankenEnum.Ciseaux && party.ChoixTwo == JankenEnum.Pierre) ||
                (party.ChoixOne == JankenEnum.Feuille && party.ChoixTwo == JankenEnum.Ciseaux) ||
                (party.ChoixOne == JankenEnum.Pierre && party.ChoixTwo == JankenEnum.Feuille))
            {
                roomuserTwo.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.win", roomuserTwo.GetClient().Langue), this.GetSringChoix(party.ChoixTwo, roomuserTwo.GetClient().Langue), this.GetSringChoix(party.ChoixOne, roomuserTwo.GetClient().Langue), roomuserOne.GetUsername()));
                roomuserOne.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.loose", roomuserOne.GetClient().Langue), this.GetSringChoix(party.ChoixTwo, roomuserOne.GetClient().Langue), this.GetSringChoix(party.ChoixOne, roomuserOne.GetClient().Langue), roomuserTwo.GetUsername()));

                EnableEffet(roomuserOne, party.ChoixOne);
                EnableEffet(roomuserTwo, party.ChoixTwo);
                return true;
            }

            return true;
        }

        private string GetSringChoix(JankenEnum Choix, Language langue)
        {
            switch(Choix)
            {
                case JankenEnum.Ciseaux:
                    return ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.ciseaux", langue);
                case JankenEnum.Feuille:
                    return ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.feuille", langue);
                case JankenEnum.Pierre:
                    return ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.pierre", langue);
                default:
                    return "";
            }
        }

        private void EnableEffet(RoomUser user, JankenEnum Janken)
        {
            if (Janken == JankenEnum.Ciseaux)
            {
                user.ApplyEffect(563, true);
            }
            else if (Janken == JankenEnum.Pierre)
            {
                user.ApplyEffect(565, true);
            }
            else if (Janken == JankenEnum.Feuille)
            {
                user.ApplyEffect(564, true);
            }

            user.TimerResetEffect = 10;
        }

        public void RemovePlayer(RoomUser User)
        {
            if (User.PartyId == 0)
                return;

            Janken party = GetParty(User.PartyId);
            if (party == null)
                return;

            if (!party.Started)
            {
                RoomUser roomuserOne = mRoom.GetRoomUserManager().GetRoomUserByHabboId(party.UserOne);
                RoomUser roomuserTwo = mRoom.GetRoomUserManager().GetRoomUserByHabboId(party.UserTwo);

                if (roomuserOne != null)
                    roomuserOne.PartyId = 0;

                if (roomuserTwo != null)
                    roomuserTwo.PartyId = 0;

                mJankenParty.TryRemove(party.UserOne, out party);
            }
        }

        public Janken GetParty(int id)
        {
            if (mJankenParty.ContainsKey(id))
                return mJankenParty[id];
            else
                return null;
        }
    }
}
