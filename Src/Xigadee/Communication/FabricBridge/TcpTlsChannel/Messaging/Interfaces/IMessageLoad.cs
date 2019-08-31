
namespace Xigadee
{
    /// <summary>
    /// This interface specifies whether the entity can be loaded.
    /// </summary>
    public interface IMessageLoad
    {
        /// <summary>
        /// This boolean property that specifies whether the message can be loaded.
        /// </summary>
        bool CanLoad { get; }
        /// <summary>
        /// This boolean property specifies whether the entity has been loaded.
        /// </summary>
        bool Loaded { get; }
    }
}
