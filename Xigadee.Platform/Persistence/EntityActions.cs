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

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is a set of shared definitions for the persistence methods.
    /// </summary>
    public static class EntityActions
    {
        public const string Create = "Create";
        public const string Read = "Read";
        public const string ReadByRef = "ReadByRef";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string DeleteByRef = "DeleteByRef";
        public const string Version = "Version";
        public const string VersionByRef = "VersionByRef";

        public const string Search = "Search";
        public const string History = "History";
    }
}
