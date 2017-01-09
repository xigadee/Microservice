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

        public bool Exists(string claimType)
        {
            JToken token;
            return jObj.TryGetValue(claimType, out token);
        }

        public void Add(string claimType, object value)
        {
            this[claimType] = value;
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

        /// <summary>
        /// This is the JSON formatted data.
        /// </summary>
        /// <returns>A JSON data format.</returns>
        public override string ToString()
        {
            return jObj.ToString(Newtonsoft.Json.Formatting.None);
        }


        protected S GetClaim<S>(string header)
        {
            return Exists(header) ? ((S)this[header]) : default(S);
        }

    }
}
