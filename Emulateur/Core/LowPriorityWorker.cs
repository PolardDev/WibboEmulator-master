using System;
using Butterfly.Database.Interfaces;
using System.Diagnostics;

namespace Butterfly.Core
{
    public class LowPriorityWorker
    {
        private static int UserPeak;
        private static bool isExecuted = false;

        private static string mColdTitle;

        public static void Init()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT userpeak FROM server_status");
                UserPeak = dbClient.GetInteger();
            }
            mColdTitle = string.Empty;

            lowPriorityProcessWatch = new Stopwatch();
            lowPriorityProcessWatch.Start();
        }


        private static Stopwatch lowPriorityProcessWatch;
        public static void Process()
        {
            if (lowPriorityProcessWatch.ElapsedMilliseconds >= 60000 || !isExecuted)
            {
                isExecuted = true;
                lowPriorityProcessWatch.Restart();
                try
                {
                    int UsersOnline = ButterflyEnvironment.GetGame().GetClientManager().Count;

                    if(ButterflyEnvironment.GetGame().GetAnimationManager().Start && UsersOnline < 150)
                    {
                        ButterflyEnvironment.GetGame().GetAnimationManager().Start = false;
                    } else if (!ButterflyEnvironment.GetGame().GetAnimationManager().Start && UsersOnline >= 150)
                    {
                        ButterflyEnvironment.GetGame().GetAnimationManager().Start = true;
                    }

                    if (UsersOnline > UserPeak)
                        UserPeak = UsersOnline;

                    int RoomsLoaded = ButterflyEnvironment.GetGame().GetRoomManager().Count;

                    TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

                   mColdTitle = "Butterfly | Uptime: " + Uptime.Days + " day " + Uptime.Hours + " hours " + Uptime.Minutes + " minutes | " +
                        "Online users: " + UsersOnline + " | Loaded rooms: " + RoomsLoaded;

                    Console.Title = mColdTitle;

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE server_status SET users_online = " + UsersOnline + ", rooms_loaded = " + RoomsLoaded + ", userpeak = " + UserPeak + ", stamp = UNIX_TIMESTAMP();INSERT INTO cms_connecter (connecter, date, appart) VALUES (" + UsersOnline + ", UNIX_TIMESTAMP(), " + RoomsLoaded + ");");
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "Server status update task"); }
            }
        }
    }
}