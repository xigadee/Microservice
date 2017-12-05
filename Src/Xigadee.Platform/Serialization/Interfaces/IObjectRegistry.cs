using System;
namespace Xigadee
{
    /// <summary>
    /// This interface can be used to pass messages between Commands within the same Microservice container without the overhead of serialization.
    /// Objects are registered through this container and then pass using the reference Ids generated.
    /// </summary>
    public interface IObjectRegistry
    {

    }
}
