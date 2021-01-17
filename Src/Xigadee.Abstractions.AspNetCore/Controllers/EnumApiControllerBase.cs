using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This controller base is used to manage a set list of enumeration for an application.
    /// </summary>
    public abstract class EnumControllerBase : ApiControllerBase
    {
        #region Constructor
        /// <summary>
        /// This is the base controller.
        /// </summary>
        /// <param name="logger">The is the logger.</param>
        protected EnumControllerBase(ILogger logger) : base(logger)
        {
        } 
        #endregion

        /// <summary>
        /// This method can be called to generate a definition based on an internal system enum definition.
        /// It is useful for sharing internal list states with an external applcation.
        /// </summary>
        /// <typeparam name="ENUM">The enum type to share.</typeparam>
        /// <param name="validity">The optional validity that the application can share this information.</param>
        /// <returns>Returns a result object with the data.</returns>
        protected IActionResult BuildEnumList<ENUM>(TimeSpan? validity = null)
            => StatusCode(StatusCodes.Status200OK, new EnumDefinition(typeof(ENUM), validity));
    }

}
