namespace Butterfly.Communication.RCON.Commands.Hotel
{
    class UpdateNavigatorCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
                ButterflyEnvironment.GetGame().GetNavigator().Init();
            return true;
        }
    }
}
