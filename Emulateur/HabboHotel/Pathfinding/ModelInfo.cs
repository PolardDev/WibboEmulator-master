namespace Butterfly.HabboHotel.Pathfinding
{
  public struct ModelInfo
  {
    private readonly byte[,] mMap;
    private readonly int mMaxX;
    private readonly int mMaxY;
    private readonly byte[,] mUserOnMap;
    private readonly byte[,] mSquareTaking;

    public ModelInfo(int MaxX, int MaxY, byte[,] Map, byte[,] UserOnMap, byte[,] SquareTaking)
    {
      this.mMap = Map;
      this.mMaxX = MaxX;
      this.mMaxY = MaxY;
      this.mUserOnMap = UserOnMap;
      this.mSquareTaking = SquareTaking;
    }

      public byte GetStateUser(int x, int y)
    {
        if (x >= this.mMaxX || x < 0 || (y >= this.mMaxY || y < 0))
            return 1;

        if (this.mUserOnMap[x, y] == 1 || this.mSquareTaking[x, y] == 1)
            return 1;
        else 
            return 0;
    }

    public byte GetState(int x, int y)
    {
      if (x >= this.mMaxX || x < 0 || (y >= this.mMaxY || y < 0))
        return (byte) 0;
      else
        return this.mMap[x, y];
    }
  }
}
