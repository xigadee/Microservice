using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class contains the claims set for the token.
    /// </summary>
    public class ClaimsSet: IEnumerable<KeyValuePair<string, object>>
    {
        private JObject jObj;

        public ClaimsSet()
        {
            jObj = new JObject();
        }

        public ClaimsSet(string json)
        {
            jObj = JObject.Parse(json);
        }


        public object this[string claimType]
        {
            get
            {
                var token = jObj[claimType];

                var value = token.ToObject<object>();

                return value;
            }
            set { jObj[claimType] = JToken.FromObject(value); }
        }


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in jObj)
            {
                yield return new KeyValuePair<string, object>(item.Key, (object)item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return jObj.ToString();
        }
    }
}
