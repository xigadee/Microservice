using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Xigadee
{
    public class PopulatorWebApiUnity<M,C>: PopulatorWebApiBase<M, C>
        where M : Microservice, new()
        where C : ConfigBase, new()
    {
        public IUnityContainer Unity { get; }

        public PopulatorWebApiUnity()
        {
            Unity = new UnityContainer();
        }

        protected virtual void RegisterCommand<I, P>() where P : I, ICommand, new()
        {
            RegisterCommand<I, P>(new P());
        }

        protected virtual void RegisterCommand<I, P>(P instance) where P : I, ICommand
        {
            try
            {
                Service.RegisterCommand(instance);
                Unity.RegisterInstance<I>(instance);
            }
            catch (Exception ex)
            {
                //Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
