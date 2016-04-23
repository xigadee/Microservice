using System;

namespace Xigadee
{
    public interface ICommandHandlerStatistics
    {
        DateTime? LastAccessed { get; set; }
        string Name { get; set; }
    }
}