#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text; 
#endregion
namespace Xigadee
{
    [KnownType("PayloadDeliveryGetKnownTypes")]
    [DataContract]
    public class PayloadDelivery
    {
        #region Known Types Static Declaration.
        static System.Type[] sKnownTypes;

        static PayloadDelivery()
        {
            var knownTypes = new List<System.Type>();
            //sKnownTypes.Add(typeof(List<KeyValuePair<string, List<string>>>));
            knownTypes.Add(typeof(string));
            knownTypes.Add(typeof(KeyValuePair<string, List<string>>));

            Func<object,bool> hasDataContract = (o) => o.GetType() == typeof(DataContractAttribute);

            var assTypes = typeof(PayloadDelivery).Assembly
                .GetTypes()
                .Where(t => t != typeof(PayloadDelivery))
                .Where(t => t.GetCustomAttributes(false).Count(hasDataContract) > 0)
                .ToList();
                
            knownTypes.AddRange(assTypes);
            //OK, add the list representation to be safe.
            var listTypes = knownTypes.Select(t => typeof(List<>).MakeGenericType(t)).ToList();

            knownTypes.AddRange(listTypes);

            sKnownTypes = knownTypes.ToArray();
        }

        static System.Type[] PayloadDeliveryGetKnownTypes()
        {
            return sKnownTypes;
        } 
        #endregion

        [DataMember]
        List<KeyValuePair<string, KeyValuePair<string, object>>> mItems = new List<KeyValuePair<string, KeyValuePair<string, object>>>();

        public void Add(object data)
        {
            Add("", data);
        }

        public void Add(string id, object data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var item = new KeyValuePair<string, object>(data.GetType().AssemblyQualifiedName, data);
            var item2 = new KeyValuePair<string, KeyValuePair<string, object>>(id, item);

            mItems.Add(item2);
        }

        public object Get()
        {
            return Get("");
        }

        public E Get<E>()
        {
            return (E)Get("");
        }

        public E Get<E>(string key)
        {
            return (E)Get(key); 
        }

        public object Get(string key)
        {
            var data = mItems.FirstOrDefault(i => i.Key == key);

            return data.Value.Value;
        }
    }
}
