using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public abstract class ParameterBase
    {
        public int Position { get; set; }

        public string Parameter { get; set; }

        protected static bool CompareOperator(string op, string value) => string.Equals(value?.Trim(), op, StringComparison.InvariantCultureIgnoreCase);

        public virtual void Load(int position, string value)
        {
            Position = position;
            Parse(value);
        }

        /// <summary>
        /// This abstract method is used to parse the string value in to the relevant properties.
        /// </summary>
        /// <param name="value"></param>
        protected abstract void Parse(string value);

    }

}
