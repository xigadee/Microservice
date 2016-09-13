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
using System.Xml.Linq;
using Xigadee;
namespace Test.Xigadee
{
    public static class MondayMorningBluesHelper
    {

        public static IEnumerable<Tuple<string, string>> ToReferences(this MondayMorningBlues entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Email))
                return new[] { new Tuple<string, string>("email", entity.Email) };

            return new Tuple<string, string>[] { };
        }

        public static XElement ToXml(this MondayMorningBlues entity)
        {
            var root = new XElement("MondayMorningBlues");

            root.Add(new XAttribute("ExternalId", entity.Id));
            root.Add(new XAttribute("ContentId", entity.ContentId));
            root.Add(new XAttribute("Email", entity.Email));
            root.Add(new XAttribute("Message", entity.Message));
            root.Add(new XAttribute("NotEnoughCoffee", entity.NotEnoughCoffee));
            root.Add(new XAttribute("NotEnoughSleep", entity.NotEnoughSleep));
            root.Add(new XAttribute("VersionId", entity.VersionId));

            return root;
        }

        public static MondayMorningBlues ToMondayMorningBlues(XElement node)
        {
            var entity = new MondayMorningBlues
            {
                Id = ConversionHelper.ToGuid(node.Attribute("ExternalId")).Value,
                VersionId = ConversionHelper.ToGuid(node.Attribute("VersionId")).Value,
                NotEnoughCoffee = ConversionHelper.ToBoolean(node.Attribute("NotEnoughCoffee"))??false,
                NotEnoughSleep = ConversionHelper.ToBoolean(node.Attribute("NotEnoughSleep"))??false,
                ContentId = ConversionHelper.ToGuid(node.Attribute("ContentId")).Value,
                Email = ConversionHelper.NodeNullable(node.Attribute("Email")),
                Message = ConversionHelper.NodeNullable(node.Attribute("Message")),
            };

            return entity;
        }

        #region Version

        public static VersionPolicy<MondayMorningBlues> VersionPolicyHelper =
            new VersionPolicy<MondayMorningBlues>(e => e.VersionId.ToString("N").ToLowerInvariant(), e => e.VersionId = Guid.NewGuid());


        public static Tuple<Guid, string> ToVersion(XElement node)
        {
            return new Tuple<Guid, string>(
                ConversionHelper.ToGuid(node.Attribute("ExternalId")) ?? Guid.Empty,
                ConversionHelper.NodeNullable(node.Attribute("VersionId")));
        }

        #endregion
    }
}
