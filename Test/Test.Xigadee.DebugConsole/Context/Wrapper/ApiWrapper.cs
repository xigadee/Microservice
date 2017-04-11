using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class ApiWrapper : WrapperBase
    {
        public override ServiceStatus Status => throw new NotImplementedException();

        public override IRepositoryAsync<Guid, MondayMorningBlues> Persistence => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        public override event EventHandler<StatusChangedEventArgs> StatusChanged;

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
