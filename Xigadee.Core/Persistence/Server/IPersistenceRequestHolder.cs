using System;
#region using
#endregion
namespace Xigadee
{
    public interface IPersistenceRequestHolder
    {
        Guid ProfileId { get; }

        int Start { get; }

        string Debug { get; }

    }
}
