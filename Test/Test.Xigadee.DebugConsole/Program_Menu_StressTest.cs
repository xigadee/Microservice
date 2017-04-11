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
        static Lazy<ConsoleMenu> sMenuServerStressTest = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                $"Server Stress Tests ({sContext.PersistenceType.ToString()})"
                , new ConsoleOption("Stress test 1"
                    , (m, o) =>
                    {
                        Task.Run(() => StressTest1());
                    }
                    , enabled: (m, o) => true
                )
                )
            );

        static void StressTest1()
        {

        }

        static Lazy<ConsoleMenu> sMenuClientStressTests = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                $"Client Stress Tests({sContext.PersistenceType.ToString()})"
                //, new ConsoleOption("Sql based"
                //    , (m, o) =>
                //    {
                //        sContext.PersistenceType = PersistenceOptions.Sql;
                //    }
                //    , enabled: (m, o) => clientStatus == 0

                //)
                , new ConsoleOption("DocumentDb based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.DocumentDb;
                        sMenuClientStressTests.Value.Refresh();
                    }
                    , enabled: (m, o) => true
                )
                , new ConsoleOption("Blob storage based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.Blob;
                        sMenuClientStressTests.Value.Refresh();
                    }
                    , enabled: (m, o) => true
                )
                , new ConsoleOption("Redis Cache based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.RedisCache;
                        sMenuClientStressTests.Value.Refresh();
                    }
                    , enabled: (m, o) => true
                )
                )
            );
    }
}
