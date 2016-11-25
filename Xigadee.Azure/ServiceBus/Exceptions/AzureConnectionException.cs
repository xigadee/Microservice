using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class AzureConnectionException:Exception
    {
        public AzureConnectionException():base("Azure connection string is null or empty.")
        {

        }
    }
}
