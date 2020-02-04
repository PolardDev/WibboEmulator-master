// Type: Butterfly.HabboHotel.Rooms.UserSaysArgs




using System;

namespace Butterfly.HabboHotel.Rooms
{
  public class UserSaysArgs : EventArgs
  {
    public readonly RoomUser user;
    public readonly string message;

    public UserSaysArgs(RoomUser user, string message)
    {
      this.user = user;
      this.message = message;
    }
  }
}
