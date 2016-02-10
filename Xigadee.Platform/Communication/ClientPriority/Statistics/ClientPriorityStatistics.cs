#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public class ClientPriorityStatistics: StatusBase
    {
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }
        public List<ClientPriorityHolderStatistics> ClientHolders { get; set; }
    }
}
