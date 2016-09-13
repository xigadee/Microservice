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

#region using
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to serialize and deserialize and validate the XML.
    /// </summary>
    public abstract class XmlTransportSerializerBase<E> : TransportSerializer<E>
    {
        public XmlTransportSerializerBase():base()
        {
            MediaType = "application/xml";
        }


        protected XmlSchema Schema { get; private set; }

        public abstract string XsdName { get; }

        protected virtual E EntityMaker(XElement xml)
        {
            var serializer = new XmlSerializer(typeof(E));
            return (E)serializer.Deserialize(xml.CreateReader());            
        }

        protected virtual XElement XmlMaker(E entity)
        {
            var doc = new XDocument();
            var serializer = new XmlSerializer(typeof(E));
            using (var writer = doc.CreateWriter())
            {
                serializer.Serialize(writer, entity);
            }

            return doc.Root;
        }

        protected byte[] ToBytes(XElement xml)
        {
            return Encoding.UTF8.GetBytes(xml.ToString());
        }

        public override E GetObjectInternal(byte[] data, Encoding encoding)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            
            XElement root;
            using (var xmlStream = new MemoryStream(data))
            {
                xmlStream.Position = 0;
                root = XElement.Load(xmlStream);           
            }

            Schema = Schema ?? GetSchema(XsdName);
            
            string errors;
            if (!XmlSchemaValidation.ValidateXmlToSchema(Schema, root, out errors))
                throw new XmlSchemaValidationException(errors);

            return EntityMaker(root);
        }

        public override byte[] GetDataInternal(E entity, Encoding encoding)
        {
            return ToBytes(XmlMaker(entity));
        }

        private XmlSchema GetSchema(string schemaName)
        {
            if (string.IsNullOrEmpty(schemaName))
                throw new ArgumentNullException("schemaName", "Schema name not supplied");

            var currentAssembly = GetType().Assembly;
            string schemaManifestName = currentAssembly.GetManifestResourceNames()
                .FirstOrDefault(rn => rn.EndsWith(string.Format(".{0}.xsd", schemaName), StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrEmpty(schemaManifestName))
                throw new ArgumentException("Unable to location schema manifest " + schemaName, "schemaName");

            return XmlSchema.Read(currentAssembly.GetManifestResourceStream(schemaManifestName), (s, e) => { });
        }
    }
}
