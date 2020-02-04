// Type: Butterfly.HabboHotel.Support.HelpTopic




namespace Butterfly.HabboHotel.Support
{
  public class HelpTopic
  {
    private readonly int Id;
    public string Caption;
    public string Body;
    public int CategoryId;

    public int TopicId
    {
      get
      {
        return this.Id;
      }
    }

    public HelpTopic(int Id, string Caption, string Body, int CategoryId)
    {
      this.Id = Id;
      this.Caption = Caption;
      this.Body = Body;
      this.CategoryId = CategoryId;
    }
  }
}
