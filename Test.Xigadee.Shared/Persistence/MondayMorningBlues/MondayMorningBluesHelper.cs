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
