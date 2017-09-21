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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class ConsoleExtensionMethods
    {
        /// <summary>
        /// This method can be called by an external process to update the info messages displayed in the menu.
        /// </summary>
        /// <param name="menu">The info message</param>
        /// <param name="pipeline">The refresh option flag.</param>
        /// <param name="useParentContextInfo">The log type.</param>
        public static ConsoleMenu AddMicroservicePipeline(this ConsoleMenu menu, IPipeline pipeline
            , bool useParentContextInfo = true)
        {
            var ms = pipeline.ToMicroservice();

            string title = $"Microservice: {ms.Id.Name}";

            var msMenu = new ConsoleMenu(title) { ContextInfoInherit = useParentContextInfo };

            ms.StatusChanged += (s,e) =>
            {
                msMenu.AddInfoMessage($"{ms.Id.Name} service status changed: {e.StatusNew}", true);
            };

            menu.OnClose += (m,e) =>
            {
                if (ms.Status == ServiceStatus.Running)
                    pipeline.Stop();
            };

            msMenu.AddOption("Start", (m, o) =>
            {
                try
                {
                    pipeline.Start();
                }
                catch (Exception ex)
                {
                    msMenu.AddInfoMessage($"{ms.Id.Name} start error: {ex.Message}", true, LoggingLevel.Error);
                }
            }, enabled:(m,o) => ms.Status != ServiceStatus.Running);

            msMenu.AddOption("Stop", (m, o) => pipeline.Stop(), enabled: (m, o) => ms.Status == ServiceStatus.Running);
            //msMenu.AddOption("View Data Collection", (m, o) => pipeline.Stop(), enabled: (m, o) => ms.Status == ServiceStatus.Running);

            //Add an option to the main menu.
            menu.AddOption(new ConsoleOption(title, msMenu));

            return menu;
        }
    }
}
