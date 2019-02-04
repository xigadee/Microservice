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
        /// <summary>
        /// The create action.
        /// </summary>
        public const string Create = "Create";
        /// <summary>
        /// The read action.
        /// </summary>
        public const string Read = "Read";
        /// <summary>
        /// The read by reference action.
        /// </summary>
        public const string ReadByRef = "ReadByRef";
        /// <summary>
        /// The update action.
        /// </summary>
        public const string Update = "Update";
        /// <summary>
        /// The delete action.
        /// </summary>
        public const string Delete = "Delete";
        /// <summary>
        /// The delete by reference action.
        /// </summary>
        public const string DeleteByRef = "DeleteByRef";
        /// <summary>
        /// The version action.
        /// </summary>
        public const string Version = "Version";
        /// <summary>
        /// The version by reference action.
        /// </summary>
        public const string VersionByRef = "VersionByRef";

        /// <summary>
        /// The search action.
        /// </summary>
        public const string Search = "Search";
        /// <summary>
        /// The history action.
        /// </summary>
        public const string History = "History";
    }
}
