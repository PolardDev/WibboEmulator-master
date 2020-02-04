using Butterfly.HabboHotel.GameClients;
using System.Threading.Tasks;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class shutdown : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Task ShutdownTask = new Task(ButterflyEnvironment.PreformShutDown);
            ShutdownTask.Start();
        }
    }
}
