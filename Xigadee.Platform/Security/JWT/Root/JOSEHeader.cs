using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class JOSEHeader:ClaimsSet
    {
        public const string HeaderType = "typ";
        public const string HeaderAlgorithm = "alg";
        public const string HeaderContentType = "cty";
        public const string HeaderJSONWebKey = "jwk";
        public const string HeaderKeyID = "kid";
        public const string HeaderJWKSetURL = "jku";
        public const string HeaderCritical = "crit";

        public JOSEHeader():base()
        {
        }

        public JOSEHeader(string json) : base(json)
        {
            
        }

        public string Type
        {
            get
            {
                return (string)this[HeaderType];
            }
            set
            {
                this[HeaderType] = value;
            }
        }

        public string ContentType
        {
            get
            {
                return (string)this[HeaderContentType];
            }
            set
            {
                this[HeaderContentType] = value;
            }
        }

        public JWTHashAlgorithm Algorithm
        {
            get
            {
                return JWTHolder.ConvertToJWTHashAlgorithm((string)this[HeaderAlgorithm]);
            }
            set
            {
                this[HeaderAlgorithm] = Enum.GetName(typeof(JWTHashAlgorithm), value);
            }
        }
    }
}
