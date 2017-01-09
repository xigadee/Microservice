using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class AuthenticationHandlerId:SecurityHandlerIdBase
    {
        public AuthenticationHandlerId(string id):base(id)
        {
        }


        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="id">The name of the resource profile.</param>
        public static implicit operator AuthenticationHandlerId(string id)
        {
            return new AuthenticationHandlerId(id);
        }
    }
}
