using System;
using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This is the helper class for the repository holder.
    /// </summary>
    public static class RepositoryHolderHelper
    {

        /// <summary>
        /// This extension method is true when the response code is '404: not found' .
        /// </summary>
        /// <param name="holder">The repository hodler response.</param>
        /// <returns>Returns true of not found.</returns>
        public static bool ResponseIsNotFound(this RepositoryHolder holder) => !holder.IsSuccess && holder.ResponseCode == (int)PersistenceResponse.NotFound404;
    }
}
