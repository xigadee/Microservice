using System;

namespace Xigadee
{
    /// <summary>
    /// Indicates that request without a client certificate can be made - allows certain actions
    /// to not pass a client certificate when it has been globally specified
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AllowNoClientCertificateAttribute : Attribute
    {
    }
}
