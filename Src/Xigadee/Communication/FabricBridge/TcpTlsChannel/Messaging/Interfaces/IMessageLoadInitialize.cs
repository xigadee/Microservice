namespace Xigadee
{
    public interface IMessageLoadInitialize
    {
        void Load();

        void Load(long maxLength);
    }

    public interface IMessageLoadInitialize<TERM>
        where TERM : IMessageTermination
    {
        void Load(TERM terminator);

        void Load(TERM terminator, long maxLength);
    }
}
