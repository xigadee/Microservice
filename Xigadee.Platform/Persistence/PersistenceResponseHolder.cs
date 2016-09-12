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
    public class PersistenceResponseHolder<E>: PersistenceResponseHolder, IResponseHolder<E>
    {
        public PersistenceResponseHolder():base()
        {

        }
        public PersistenceResponseHolder(PersistenceResponse? status = null, string content = null, E entity = default(E)):base(status, content)
        {
            Entity = entity;
        }

        public E Entity
        {
            get; set;
        }
    }

    public class PersistenceResponseHolder: IResponseHolder
    {
        public PersistenceResponseHolder(PersistenceResponse? status = null, string content = null):this()
        {
            if (status.HasValue)
            {
                StatusCode = (int)status.Value;
                IsSuccess = StatusCode>=200 && StatusCode <= 299;

                IsTimeout = !IsSuccess && StatusCode == 408;
            }
            Content = content;
        }

        public PersistenceResponseHolder()
        {
            Fields = new Dictionary<string, string>();
        }

        public string Content
        {
            get;set;
        }


        public Exception Ex
        {
            get;set;
        }

        public Dictionary<string, string> Fields
        {
            get;set;
        }

        public string Id
        {
            get;set;
        }

        public bool IsSuccess
        {
            get;set;
        }

        public bool IsTimeout
        {
            get;set;
        }

        public bool IsCacheHit
        {
            get;set;
        }

        public int StatusCode
        {
            get;set;
        }

        public string VersionId
        {
            get;set;
        }

    }
}
