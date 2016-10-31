using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the token held by the Dispatcher when a request is being processed.
    /// </summary>
    public class DispatcherSecurityToken: SecurityToken
    {
        public override string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override DateTime ValidFrom
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
