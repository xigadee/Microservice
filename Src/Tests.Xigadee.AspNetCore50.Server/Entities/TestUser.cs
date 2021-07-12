using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50
{
    public class TestUser:User
    {
        public const string KeyUsername = "username";
        public const string KeyEmail = "email";

        [JsonIgnore]
        [EntityReferenceHint(KeyUsername)]
        public string Username
        {
            get => Properties.GetValueOrDefault(KeyUsername);
            set => Properties[KeyUsername] = value;
        }

        [JsonIgnore]
        [EntityReferenceHint(KeyEmail)]
        public string Email
        {
            get => Properties.GetValueOrDefault(KeyEmail);
            set => Properties[KeyEmail] = value;
        }
    }
}
