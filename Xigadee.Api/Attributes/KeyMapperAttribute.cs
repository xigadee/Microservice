using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This key is used to specify a key mapper for a specific key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class KeyMapperAttribute : Attribute
    {
        public KeyMapperAttribute(Type keyMapper)
        {
            if (keyMapper == null)
                throw new ArgumentNullException("keyMapper");

            if (!keyMapper.IsSubclassOf(typeof(KeyMapper)))
                throw new ArgumentOutOfRangeException("keyMapper", "Type should be a subclass of KeyMapper");

            KeyMapper = keyMapper;
        }

        public Type KeyMapper { get; private set; }
    }
}
