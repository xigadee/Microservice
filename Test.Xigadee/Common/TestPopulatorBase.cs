#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        where M : Microservice, new()
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
