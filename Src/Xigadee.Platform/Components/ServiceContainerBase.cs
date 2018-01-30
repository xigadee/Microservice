namespace Xigadee
{
    /// <summary>
    /// This is the abstract class used by the primary Microservice containers.
    /// </summary>
    /// <typeparam name="S">The status class.</typeparam>
    /// <typeparam name="P">The policy class.</typeparam>
    public abstract class ServiceContainerBase<S,P>:ServiceBase<S>
        where S : StatusBase, new()
        where P : PolicyBase, new()
    {
        /// <summary>
        /// This is the container policy.
        /// </summary>
        protected P mPolicy;

        /// <summary>
        /// This is the default construct that sets or creates the policy object depending on whether it is passed in to the constructor. 
        /// </summary>
        /// <param name="policy">The optional policy class.</param>
        /// <param name="name">The optional name for the component.</param>
        public ServiceContainerBase(P policy = null, string name = null):base(name)
        {
            mPolicy = policy ?? new P();
        }
    }
}
