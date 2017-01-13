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


namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        /// <summary>
        /// This method adds the Api version number to the response header. 
        /// This can be used for debugging.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="webpipe"></param>
        /// <param name="headerName">The HTTP header name, which by default is X-XigadeeApiVersion</param>
        /// <returns>Returns the pipe.</returns>
        public static P AddVersionHeaderFilter<P>(this P webpipe, string headerName = "X-XigadeeApiVersion")
            where P : IPipelineWebApi
        {
            var filter = new WebApiVersionHeaderFilter(headerName);

            webpipe.HttpConfig.Filters.Add(filter);

            return webpipe;
        }
    }

}
