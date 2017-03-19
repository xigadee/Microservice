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

using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMenuServerPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                "Persistence"
                , Create(sContext.ApiServer)
                , Read(sContext.ApiServer)
                , ReadByReference(sContext.ApiServer)
                , Update(sContext.ApiServer)
                , Delete(sContext.ApiServer)
                , DeleteByReference(sContext.ApiServer)
                , Version(sContext.ApiServer)
                , VersionByReference(sContext.ApiServer)
                , Search(sContext.ApiServer)
                , StressTest(sContext.ApiServer)
                , StressCrudTest(sContext.ApiServer)
               )
            );
    }
}
