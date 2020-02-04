namespace Butterfly.HabboHotel.Pathfinding
{
  public struct HeightInfo
  {
    private readonly double[,] mMap;
    private readonly int mMaxX;
    private readonly int mMaxY;

    public HeightInfo(int MaxX, int MaxY, double[,] Map)
    {
      this.mMap = Map;
      this.mMaxX = MaxX;
      this.mMaxY = MaxY;
    }

    public double GetState(int x, int y)
    {
      if (x >= this.mMaxX || x < 0 || (y >= this.mMaxY || y < 0))
        return 0.0;
      else
        return this.mMap[x, y];
    }
  }
}
