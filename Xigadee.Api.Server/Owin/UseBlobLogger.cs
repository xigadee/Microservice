using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Xigadee
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ApiExtensionUseBlobLogger
    {
        public static IAppBuilder UseBlobLogger(this IAppBuilder builder)
        {
            return builder;
        }
    }
}
