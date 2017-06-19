using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public abstract class WrapperBase<K,E> : IConsolePersistence<K,E>
        where K : IEquatable<K>
    {

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public abstract ServiceStatus Status { get; }

        public IRepositoryAsync<K, E> Persistence { get {return PersistenceClient ?? PersistenceServer; } }
        public IRepositoryAsync<K, E> PersistenceClient { get; set; }
        public IRepositoryAsync<K, E> PersistenceServer { get; set; }

        public abstract string Name { get; protected set; }


        public abstract void Start();

        public abstract void Stop();

        protected void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(sender, e);
        }

    }
}
