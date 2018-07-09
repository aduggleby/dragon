namespace Dragon.CPR.Interfaces
{
    public interface ICommandSerializer
    {
        string Serialize(CommandBase command);
        object Deserialize(Command command);
    }
}
