using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestUser:User
    {
        [JsonIgnore]
        public string Username
        {
            get => Properties.GetValueOrDefault("username");
            set => Properties[Username] = value;
        }
    }
}
