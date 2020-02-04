namespace Butterfly.HabboHotel.Pathfinding
{
  public struct SquareInformation
  {
    private readonly int mX;
    private readonly int mY;
    private readonly SquarePoint[] mPos;
    private readonly SquarePoint mTarget;
    private readonly SquarePoint mPoint;

    public SquarePoint Point
    {
      get
      {
        return this.mPoint;
      }
    }

    public SquareInformation(int pX, int pY, SquarePoint pTarget, ModelInfo pMap, bool pUserOverride, bool CalculateDiagonal, bool pAllowWalkthrough, bool DisableOblique)
    {
      this.mX = pX;
      this.mY = pY;
      this.mTarget = pTarget;
      this.mPoint = new SquarePoint(pX, pY, pTarget.X, pTarget.Y, pMap.GetState(pX, pY), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX, pY));
      this.mPos = new SquarePoint[8];
      if (CalculateDiagonal)
      {
          this.mPos[1] = new SquarePoint(pX - 1, pY - 1, pTarget.X, pTarget.Y, pMap.GetState(pX - 1, pY - 1), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX - 1, pY - 1));
          this.mPos[3] = new SquarePoint(pX - 1, pY + 1, pTarget.X, pTarget.Y, pMap.GetState(pX - 1, pY + 1), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX - 1, pY + 1));
          this.mPos[5] = new SquarePoint(pX + 1, pY + 1, pTarget.X, pTarget.Y, pMap.GetState(pX + 1, pY + 1), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX + 1, pY + 1));
          this.mPos[7] = new SquarePoint(pX + 1, pY - 1, pTarget.X, pTarget.Y, pMap.GetState(pX + 1, pY - 1), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX + 1, pY - 1));
      }
      if (DisableOblique)
      {
          this.mPos[0] = new SquarePoint(pX, pY - 1, pTarget.X, pTarget.Y, pMap.GetState(pX, pY - 1), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX, pY - 1));
          this.mPos[2] = new SquarePoint(pX - 1, pY, pTarget.X, pTarget.Y, pMap.GetState(pX - 1, pY), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX - 1, pY));
          this.mPos[4] = new SquarePoint(pX, pY + 1, pTarget.X, pTarget.Y, pMap.GetState(pX, pY + 1), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX, pY + 1));
          this.mPos[6] = new SquarePoint(pX + 1, pY, pTarget.X, pTarget.Y, pMap.GetState(pX + 1, pY), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pX + 1, pY));
      }
    }

    public SquarePoint Pos(int val)
    {
      return this.mPos[val];
    }
  }
}
