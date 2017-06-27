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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Xigadee
{
    public static class AzureHelper
    {
        public readonly static Func<string, string, string> Resolver = (k, v) =>
        {
            string value = null;

            try
            {
                if (k != null)
                {
                    value = CloudConfigurationManager.GetSetting(k);

                    if (value == null)
                        value = ConfigurationManager.AppSettings[k];
                }
            }
            catch (Exception)
            {
                // Unable to retrieve from azure
                return null;
            }

            return value;
        };
    }
}
