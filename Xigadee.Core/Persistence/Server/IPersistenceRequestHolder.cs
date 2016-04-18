using System;
using System.Collections.Generic;
#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public interface IPersistenceRequestHolder
    {
        Guid profileId { get; }

        int start { get; }

        string Profile { get; }
    }
}
