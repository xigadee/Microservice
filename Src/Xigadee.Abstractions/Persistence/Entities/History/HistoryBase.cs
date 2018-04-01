using System;

namespace Xigadee
{
    /// <summary>
    /// The default class to contain details of an entity's history.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    public abstract class HistoryBase<K>
        where K: IEquatable<K>
    {
    }
}
