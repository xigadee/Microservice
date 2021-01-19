using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class provides a set of static helper and extension methods for the Api Provider
    /// </summary>
    public static class ApiProviderHelper
    {
        #region ToErrorObject(HttpContent c)
        /// <summary>
        /// This method deserializes the standard error object.
        /// </summary>
        /// <param name="c">The incoming HttpContent</param>
        /// <returns>Returns the deserialized error object.</returns>
        public static async Task<object> ToErrorObject(HttpContent c)
        {
            ErrorMessage res = null;
            try
            {
                res = await c.FromJsonUTF8<ErrorMessage>();
            }
            catch (Exception ex)
            {
                res = new ErrorMessage();
                res.Description = $"Deserialization error: {ex.Message}";
            }

            return (object)res;
        }
        #endregion

    }
}
