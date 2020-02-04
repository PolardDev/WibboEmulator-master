namespace Butterfly.HabboHotel.Pathfinding
{
    public struct SquarePoint
  {
    private readonly int mX;
    private readonly int mY;
    private readonly double mDistance;
    private readonly byte mSquareData;
    private readonly bool mOverride;
    private readonly bool mLastStep;
    private readonly bool mAllowWalkthrough;
    private readonly byte mSquareDataUser;

    public int X
    {
      get
      {
        return this.mX;
      }
    }

    public int Y
    {
      get
      {
        return this.mY;
      }
    }

    public double GetDistance
    {
      get
      {
        return this.mDistance;
      }
    }

    public bool CanWalk
    {
      get
      {
          if (this.mLastStep)
              return this.mOverride || this.mSquareData == 3 || this.mSquareData == 1;
          else
              return this.mOverride || this.mSquareData == 1;
      }
    }

    public bool AllowWalkthrough
    {
      get
      {
          return this.mAllowWalkthrough || mSquareDataUser == 0;
      }
    }

    public SquarePoint(int pX, int pY, int pTargetX, int pTargetY, byte SquareData, bool pOverride, bool pAllowWalkthrough, byte SquareDataUser)
    {
      this.mX = pX;
      this.mY = pY;
      this.mSquareData = SquareData;
      this.mSquareDataUser = SquareDataUser;
      this.mOverride = pOverride;
      this.mDistance = 0.0;
      this.mLastStep = pX == pTargetX && pY == pTargetY;
      this.mDistance = DreamPathfinder.GetDistance(pX, pY, pTargetX, pTargetY);
      this.mAllowWalkthrough = pAllowWalkthrough;
    }
  }
}
