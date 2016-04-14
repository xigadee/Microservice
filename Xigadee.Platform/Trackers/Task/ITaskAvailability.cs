namespace Xigadee
{
    public interface ITaskAvailability
    {
        int Level(int priority);

        int LevelMin { get; }

        int LevelMax { get; }
    }
}
