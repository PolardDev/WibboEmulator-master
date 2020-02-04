// Type: Butterfly.HabboHotel.Rooms.UserWalksOnArgs




using System;

namespace Butterfly.HabboHotel.Rooms
{
  public class UserWalksOnArgs : EventArgs
  {
    public readonly RoomUser user;

    public UserWalksOnArgs(RoomUser user)
    {
      this.user = user;
    }
  }
}
