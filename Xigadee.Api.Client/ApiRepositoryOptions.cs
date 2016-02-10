using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Xigadee.Api
{
    /// <summary>
    /// This class inherits from RespositoryOptions and pretty much does the same thing.
    /// It is currently reserved to allow for extended properties to be passed to the request
    /// to enable the fine tuning of the API calls, specifically areas relating to trace headers
    /// and the subsequent response.
    /// </summary>
    public class ApiRepositoryOptions : RepositoryOptions
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ApiRepositoryOptions()
            : base()
        {

        }
    }
}
