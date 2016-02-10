using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee.Api
{
    public class ApimIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get
            {
                return "APIMKEY";
            }
        }

        public string Id
        {
            get; set;
        }


        public bool IsAuthenticated
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Source
        {
            get; set;
        }
    }
}
