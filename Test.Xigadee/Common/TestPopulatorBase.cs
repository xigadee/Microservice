using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This class utilises the populator to correctly set up the microservice.
    /// </summary>
    /// <typeparam name="M">The microservice type.</typeparam>
    /// <typeparam name="C">The config type.</typeparam>
    public abstract class TestPopulator<M, C>: PopulatorBase<M, C>
        where M : MicroserviceBase, new()
        where C : ConfigBase, new()
    {

        /// <summary>
        /// This method ensures that the populator is started correctly
        /// </summary>
        [TestInitialize]
        public virtual void Initialise()
        {
            Populate();

            Start();
        }

        /// <summary>
        /// This override assigns the cleanup for the populator.
        /// </summary>
        [TestCleanup]
        public override void Stop()
        {
            base.Stop();
        }

    }
}
