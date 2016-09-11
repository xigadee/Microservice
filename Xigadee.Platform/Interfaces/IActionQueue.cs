using System.Threading.Tasks;

namespace Xigadee
{
    public interface IActionQueue
    {
        long OverloadProcessHits { get; }

        bool Overloaded { get; }

        int OverloadProcessCount { get; }

        int QueueLength { get; }

        Task<int> OverloadProcess();
    }
}