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
using Xigadee;

namespace Test.Xigadee
{
    [MediaTypeConverter(typeof(JsonTransportSerializer<MondayMorningBlues>))]
    public class MondayMorningBlues
    {
        public MondayMorningBlues()
        {
            Id = Guid.NewGuid();
            ContentId = Guid.NewGuid();
            VersionId = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public bool NotEnoughCoffee { get; set; }

        public bool NotEnoughSleep { get; set; }

        public Guid ContentId { get; set; }

        public Guid VersionId { get; set; }

        public string Message { get; set; }

        public string Email { get; set; }

        public Name UserName { get; set; }
    }

    public class Name
    {
        public string NameFirst { get; set; }
        public string NameFamily { get; set; }
    }
}
