// Type: Butterfly.HabboHotel.Support.ModerationBanException




using System;

namespace Butterfly.HabboHotel.Support
{
  [Serializable]
  public class ModerationBanException : Exception
  {
    public ModerationBanException(string Reason)
      : base(Reason)
    {
    }
  }
}
