namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by classes that provide repository options.
    /// </summary>
    public interface IRequestOptions
    {
        /// <summary>
        /// The repository options.
        /// </summary>
        RepositorySettings Options { get; set; }
    }
}
