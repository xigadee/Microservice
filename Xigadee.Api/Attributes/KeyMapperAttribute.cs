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
