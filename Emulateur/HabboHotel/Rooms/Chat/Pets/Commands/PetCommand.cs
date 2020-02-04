// Type: Butterfly.HabboHotel.Pets.PetCommand




namespace Butterfly.HabboHotel.Rooms.Chat.Pets.Commands
{
  public struct PetCommand
  {
    public readonly int commandID;
    public readonly string commandInput;

    public PetCommand(int commandID, string commandInput)
    {
      this.commandID = commandID;
      this.commandInput = commandInput;
    }
  }
}
