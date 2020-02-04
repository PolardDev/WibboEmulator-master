// Type: Butterfly.HabboHotel.Support.HelpCategory




namespace Butterfly.HabboHotel.Support
{
  public class HelpCategory
  {
    private readonly int Id;
    public string Caption;

    public int CategoryId
    {
      get
      {
        return this.Id;
      }
    }

    public HelpCategory(int Id, string Caption)
    {
      this.Id = Id;
      this.Caption = Caption;
    }
  }
}
