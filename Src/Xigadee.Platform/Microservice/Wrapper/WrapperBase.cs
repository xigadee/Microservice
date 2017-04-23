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

namespace Xigadee
{
    internal abstract class WrapperBase
    {
        private Func<ServiceStatus> Status;

        internal WrapperBase(Func<ServiceStatus> getStatus)
        {
            Status = getStatus;
        }

        #region ValidateServiceStarted()
        /// <summary>
        /// This method checks whether the system is running.
        /// </summary>
        protected virtual void ValidateServiceStarted()
        {
            //TODO: check whether the system is up and running.
            if (Status() != ServiceStatus.Running)
                throw new ServiceNotStartedException();
        }
        #endregion
        #region ValidateServiceNotStarted()
        /// <summary>
        /// This method checks whether the system is running.
        /// </summary>
        protected virtual void ValidateServiceNotStarted()
        {
            //TODO: check whether the system is up and running.
            switch (Status())
            {
                case ServiceStatus.Created:
                case ServiceStatus.Stopped:
                    return;
                default:
                    throw new ServiceAlreadyStartedException();
            }
        }
        #endregion

    }
}
