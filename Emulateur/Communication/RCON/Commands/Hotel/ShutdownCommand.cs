namespace Butterfly.Communication.RCON.Commands.Hotel
{
    class ShutdownCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            ButterflyEnvironment.PreformShutDown(true);
            return true;
        }
    }
}
