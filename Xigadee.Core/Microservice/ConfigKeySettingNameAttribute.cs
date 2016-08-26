using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigSettingKeyAttribute:Attribute
    {
        public ConfigSettingKeyAttribute(string category = null)
        {
            Category = category;
        }

        public string Category { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ConfigSettingAttribute: Attribute
    {
        public ConfigSettingAttribute(string category = null)
        {
            Category = category;
        }

        public string Category { get; }
    }
}
