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
    public class ClientPriorityCollectionStatistics: StatusBase
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

        public string Algorithm { get; set; }

        public List<ClientPriorityHolderStatistics> Clients { get; set; }
    }
}
