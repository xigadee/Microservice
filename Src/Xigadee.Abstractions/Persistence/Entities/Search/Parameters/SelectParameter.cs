using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class SelectParameter : ParameterBase
    {
        protected override void Parse(string value)
        {
            Parameter = value?.Trim();
        }
    }
}
