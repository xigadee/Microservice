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
using Xigadee;

namespace Xigadee
{
    public static partial class ConsolePipelineExtensions
    {
        /// <summary>
        /// This method adds an override setting and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="title">The alternate title.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P StartWithConsole<P>(this P pipeline, string title = null)
            where P : IPipeline
        {
            var ms = pipeline.ToMicroservice();

            Lazy<ConsoleMenu> mainMenu = new Lazy<ConsoleMenu>(
                () => new ConsoleMenu(title ?? $"Xigadee Test Console: {ms.Id.Name}"
                , new ConsoleOption(
                    "Run Microservice"
                    , (m, o) =>
                    {
                        ms.Start();
                    }
                    , enabled: (m, o) => ms.Status == ServiceStatus.Stopped
                )
                , new ConsoleOption(
                    "Stop Microservice"
                    , (m, o) =>
                    {
                        ms.Stop();
                    }
                    , enabled: (m, o) => ms.Status == ServiceStatus.Running
                )
                ));

            mainMenu.Value.Show();

            return pipeline;
        }
    }
}
