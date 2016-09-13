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
    public class TcpTlsClientHolder: ClientHolder<TcpTlsConnection, TcpTlsMessage>
    {
        public override void MessageComplete(TransmissionPayload payload)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            throw new NotImplementedException();
        }

        public override Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            throw new NotImplementedException();
        }
    }
}
