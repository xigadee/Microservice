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
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMenuServerPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                "Persistence"
                    , Create(sServer)
                    , Read(sServer)
                    , ReadByReference(sServer)
                    , Update(sServer)
                    , Delete(sServer)
                    , DeleteByReference(sServer)
                    , Version(sServer)
                    , VersionByReference(sServer)
                    , Search(sServer)
                    , StressTest(sServer)
                    , StressCrudTest(sServer)
               )
            );

        static Lazy<ConsoleMenu> sMenuClientPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sClient)
                    , Read(sClient)
                    , ReadByReference(sClient)
                    , Update(sClient)
                    , Delete(sClient)
                    , DeleteByReference(sClient)
                    , Version(sClient)
                    , VersionByReference(sClient)
                    , StressTest(sClient)
                   )
                );

        static Lazy<ConsoleMenu> sMenuApiPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sApiServer)
                    , Read(sApiServer)
                    , ReadByReference(sApiServer)
                    , Update(sApiServer)
                    , Delete(sApiServer)
                    , DeleteByReference(sApiServer)
                    , Version(sApiServer)
                    , VersionByReference(sApiServer)
                    , StressTest(sApiServer)

                   )
                );


    }
}
