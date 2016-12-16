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
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The security container class contains all the components to secure the incoming messaging for a Microservice, 
    /// and to ensure that incoming message requests have the correct permissions necessary to be processed.
    /// </summary>
    public partial class SecurityContainer: ServiceContainerBase<SecurityStatistics, SecurityPolicy>
        , ISecurityService, IRequireDataCollector, IServiceOriginator
    {
        #region Declarations
        /// <summary>
        /// These are the handlers used to encrpyt and decrypt blob payloads
        /// </summary>
        private Dictionary<string, IEncryptionHandler> mEncryptionHandlers;
        #endregion        
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The security policy.</param>
        public SecurityContainer(SecurityPolicy policy) : base(policy)
        {
            mEncryptionHandlers = new Dictionary<string, IEncryptionHandler>();
        } 
        #endregion
        #region Collector
        /// <summary>
        /// This is teh data collector used for logging.
        /// </summary>
        public IDataCollection Collector
        {
            get; set;
        }
        #endregion

        public bool HasEncryptionHandler(string identifier)
        {
            return mEncryptionHandlers.ContainsKey(identifier);
        }

        /////
        //public IEnumerable<KeyValuePair<string, IEncryptionHandler>> EncryptionHandlers()
        //{
        //    return mEncryptionHandlers;
        //}


        public void RegisterEncryptionHandler(string identifier, IEncryptionHandler handler)
        {
            try
            {
                mEncryptionHandlers.Add(identifier, handler);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region OriginatorId
        /// <summary>
        /// This is the system information.
        /// </summary>
        public MicroserviceId OriginatorId
        {
            get; set;
        } 
        #endregion

        protected override void StartInternal()
        {
            
        }

        protected override void StopInternal()
        {

        }
    }
}
