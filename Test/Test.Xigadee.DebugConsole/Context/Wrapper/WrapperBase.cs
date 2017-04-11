using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public abstract class WrapperBase : IConsolePersistence
    {
        public abstract ServiceStatus Status { get; }
        public abstract IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get; }
        public abstract string Name { get; }

        public abstract event EventHandler<StatusChangedEventArgs> StatusChanged;

        public abstract void Start();
        public abstract void Stop();
    }
}
