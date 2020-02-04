using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.RCON.Commands.User
{
    class SignoutCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
                return false;
            int Userid = 0;
            if (!int.TryParse(parameters[1], out Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
                return false;

            Client.Disconnect();
            return true;
        }
    }
}
