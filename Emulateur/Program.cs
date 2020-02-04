using Butterfly.Core;
using System;
using System.Security.Permissions;

namespace Butterfly
{
    public class Program
    {
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void Main()
        {
            InitEnvironment();
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("Command> ");
                    string Input = Console.ReadLine();

                    if (Input.Length > 0)
                    {
                        string s = Input.Split(' ')[0];

                        ConsoleCommands.InvokeCommand(s);
                    }
                }
            }
        }

        [MTAThread]
        public static void InitEnvironment()
        {
            Console.ForegroundColor = ConsoleColor.White;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            ButterflyEnvironment.Initialize();
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Logging.DisablePrimaryWriting(true);
            Logging.LogCriticalException("SYSTEM CRITICAL EXCEPTION: " + ((Exception)args.ExceptionObject).ToString());
            ButterflyEnvironment.PreformShutDown(true);
        }
    }
}
