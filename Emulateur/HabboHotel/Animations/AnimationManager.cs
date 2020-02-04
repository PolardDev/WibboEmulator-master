using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Animations
{
    public class AnimationManager
    {
        private List<int> _roomId;
        private bool _started;
        private int _timerStart;
        private int _roomIdGame;

        public bool Start { get; set; }
        public bool ForceDisabled { get; set; }
        public int CycleId;

        public AnimationManager()
        {
            this._roomId = new List<int>();
            this._started = false;
            this._timerStart = 0;
            this._roomIdGame = 0;
            this.Start = true;
            this.ForceDisabled = false;
        }

        public bool AllowAnimation()
        {
            if (this._started)
                return false;

            this._timerStart = 0;
            return true;
        }

        public void Init()
        {
            this._roomId.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM rooms WHERE owner = 'WibboGame'");
                DataTable table = dbClient.GetTable();
                if (table == null)
                    return;

                foreach (DataRow dataRow in table.Rows)
                {
                    if(!this._roomId.Contains(Convert.ToInt32(dataRow["id"])))
                        this._roomId.Add(Convert.ToInt32(dataRow["id"]));
                }
            }
        }

        private void AnimationCycle()
        {
            if (!Start && !this._started)
                return;
            if (ForceDisabled && !this._started)
                return;

            this._timerStart++;

            if(this._started)
            {
                if (this._timerStart >= (60 * 1) * 2)
                {
                    Room Rooom = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(this._roomIdGame);
                    this._started = false;
                    if (Rooom != null)
                        Rooom.RoomData.State = 1;
                }
                return;
            }

            if (this._timerStart > (60 * 15) * 2)
            {
                if (this.CycleId >= this._roomId.Count)
                {
                    this.CycleId = 0;
                    this._roomId = this._roomId.OrderBy(a => Guid.NewGuid()).ToList();
                }

                int RoomId = this._roomId[this.CycleId]; //ButterflyEnvironment.GetRandomNumber(0, this._roomId.Count - 1)
                this.CycleId++;

                Room room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                if (room == null)
                    return;

                this._timerStart = 0;
                this._started = true;
                this._roomIdGame = RoomId;

                room.RoomData.State = 0;
                room.CloseFullRoom = true;

                string AlertMessage = "<i>Beep beep, c'est l'heure de l'animation auto !</i>" +
                "\r\r" +
                "Rejoins-nous chez <b>WibboGame</b> pour un jeu qui s'intitule <b>" + Encoding.UTF8.GetString(Encoding.GetEncoding("Windows-1252").GetBytes(room.RoomData.Name)) + "</b>" +
                "\r\r" +
                "Rends-toi dans l'appartement et tente de remporter un lot composé de <i> un ou plusieurs Extrabox(s) ainsi qu'un point au TOP Gamer ! </i>" +
                "\r\n" +
                "\r\n- Jack et Daisy\r\n";

                ButterflyEnvironment.GetGame().GetModerationTool().LogStaffEntry(1953042, "WibboGame", room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", AlertMessage));
                ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("gameauto", "Message des AnimBots", AlertMessage, "Je veux y jouer !", room.Id, ""));
            }
        }

        public void OnCycle(Stopwatch moduleWatch)
        {
            this.AnimationCycle();
            HandleFunctionReset(moduleWatch, "AnimationCycle");
        }

        private void HandleFunctionReset(Stopwatch watch, string methodName)
        {
            try
            {
                if (watch.ElapsedMilliseconds > 500)
                    Console.WriteLine("High latency in {0}: {1}ms", methodName, watch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Canceled operation {0}", e);

            }
            watch.Restart();
        }

    }
}
