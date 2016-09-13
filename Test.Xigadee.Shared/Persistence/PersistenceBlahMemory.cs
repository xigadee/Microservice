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
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceBlahMemory: PersistenceManagerHandlerMemory<Guid, Blah>
    {
        public PersistenceBlahMemory(
              VersionPolicy<Blah> versionPolicy = null
            , ICacheManager<Guid, Blah> cacheManager = null)
            : base(
              (k) => k.ContentId
            , (s) => new Guid(s)
            , versionPolicy: versionPolicy
            //, referenceMaker: MondayMorningBluesHelper.ToReferences
            , cacheManager: cacheManager)
        {

        }

        protected override async Task ProcessSearch(PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            try
            {
                holder.Rs.ResponseCode = (int)PersistenceResponse.Ok200;
                holder.Rs.Entity = new SearchResponse();
            }
            catch (Exception ex)
            {
                holder.Rs.ResponseCode = (int)PersistenceResponse.UnknownError500;
                holder.Rs.ResponseMessage = ex.Message;
            }
        }

        public override void PrePopulate()
        {
            EntityPopulate(new Guid("3211c71a-24e5-474d-b35d-9e2f72cafbe8"), new Blah() { ContentId = new Guid("3211c71a-24e5-474d-b35d-9e2f72cafbe8"), Message = "Hello mom 32.", VersionId = Guid.NewGuid() });
            EntityPopulate(new Guid("3111c71a-24e5-474d-b35d-9e2f72cafbe8"), new Blah() { ContentId = new Guid("3111c71a-24e5-474d-b35d-9e2f72cafbe8"), Message = "Hello mom 31.", VersionId = Guid.NewGuid() });
            EntityPopulate(new Guid("3011c71a-24e5-474d-b35d-9e2f72cafbe8"), new Blah() { ContentId = new Guid("3011c71a-24e5-474d-b35d-9e2f72cafbe8"), Message = "Hello mom 30.", VersionId = Guid.NewGuid() });
            EntityPopulate(new Guid("2911c71a-24e5-474d-b35d-9e2f72cafbe8"), new Blah() { ContentId = new Guid("2911c71a-24e5-474d-b35d-9e2f72cafbe8"), Message = "Hello mom 29.", VersionId = Guid.NewGuid() });
            EntityPopulate(new Guid("2811c71a-24e5-474d-b35d-9e2f72cafbe8"), new Blah() { ContentId = new Guid("2811c71a-24e5-474d-b35d-9e2f72cafbe8"), Message = "Hello mom 28.", VersionId = Guid.NewGuid() });
        }
    }
}
