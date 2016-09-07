using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class PopulatorExtensions
    {
        Microservice mService;
        /// <summary>
        /// This method ensures that the populator is started correctly
        /// </summary>
        [TestInitialize]
        public void Initialise()
        {
            mService = new Microservice();


            //mService.
            mService.Start();
        }

        /// <summary>
        /// This override assigns the cleanup for the populator.
        /// </summary>
        [TestCleanup]
        public void Stop()
        {
            mService.Stop();
        }
    }
}
