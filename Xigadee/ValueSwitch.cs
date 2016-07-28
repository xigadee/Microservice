using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This structure allows a value to be switched off but the underlying value is retained.
    /// </summary>
    /// <typeparam name="O">The underlying value.</typeparam>
    public struct ValueSwitch<O>
    {
        /// <summary>
        /// This static method returns an unset default value for the type.
        /// </summary>
        public static ValueSwitch<O> Default
        {
            get
            {
                return new ValueSwitch<O>();
            }
        }

        /// <summary>
        /// THe default constructor.
        /// </summary>
        /// <param name="active">The default state: false</param>
        /// <param name="value">The default value for the struct or class.</param>
        public ValueSwitch(bool active = false, O value = default(O))
        {
            Active = active;
            Value = value;
        }

        /// <summary>
        /// This boolean value allowing the underlying value to be ignored but retained.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// This is the underlying value that is retained if the value is deselected.
        /// </summary>
        public O Value { get; set; }
    }
}
