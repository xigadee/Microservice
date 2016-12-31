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

namespace Xigadee
{
    /// <summary>
    /// This interface defines the Microservice definition.
    /// </summary>
    public interface IMicroservice: IService
    {
        MicroserviceId Id { get; }

        ServiceStatus Status { get; }

        IMicroserviceEvents Events { get; }

        IMicroserviceSecurity Security { get; }

        IMicroserviceCommunication Communication { get; }

        IMicroserviceDispatch Dispatch { get; }

        IMicroserviceCommand Commands { get; }

        IMicroservicePolicy Policy { get; }

        IMicroserviceDataCollection DataCollection { get; }

        IMicroserviceSerialization Serialization { get; }
    }
}