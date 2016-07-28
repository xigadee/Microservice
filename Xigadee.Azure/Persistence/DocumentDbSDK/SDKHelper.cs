using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Xigadee
{
    public static class DocumentDbSDKHelper
    {

        public static DocumentClient ToDocumentClient(this DocumentDbConnection conn, ConnectionPolicy policy = null, ConsistencyLevel? level = null)
        {
            return new DocumentClient(conn.Account, conn.AccountKey, policy??ConnectionPolicy.Default, level);
        }
    }
}
