namespace Xigadee
{
    public interface ICommandHandlerStatistics
    {
        string LastAccessed { get; set; }

        string Name { get; set; }
    }
}