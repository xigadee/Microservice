#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
