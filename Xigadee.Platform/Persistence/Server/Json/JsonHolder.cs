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

#endregion
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the root entity holder.
    /// </summary>
    public class JsonHolder
    {
        public JsonHolder(string Version, string Json, string Id = null)
        {
            this.Version = Version;
            this.Json = Json;
            this.Id = Id;
        }

        public string Id { get; set; }

        public string Version { get; set; }

        public string Json { get; set; }

        public byte[] ToBlob()
        {
            return Json == null?(byte[])null: Encoding.UTF8.GetBytes(Json);
        }
    }

    /// <summary>
    /// This class holds the Json data for persistence managers that use it as the base store.
    /// </summary>
    /// <typeparam name="KT">The key type.</typeparam>
    public class JsonHolder<KT>: JsonHolder
    {
        public JsonHolder(KT Key, string Version, string Json, string Id = null):base(Version, Json, Id)
        {
            this.Key = Key;

        }

        /// <summary>
        /// This is the key.
        /// </summary>
        public KT Key { get; set; }
    }

    /// <summary>
    /// This overload also supports operations that require the entity to be passed.
    /// </summary>
    /// <typeparam name="KT">The key type.</typeparam>
    /// <typeparam name="ET">The entity type.</typeparam>
    public class JsonHolder<KT, ET> : JsonHolder<KT>
    {
        public JsonHolder(ET Entity, KT Key, string Version, string Json, string Id = null):base(Key, Version, Json, Id)
        {
            this.Entity = Entity;
        }

        /// <summary>
        /// This is the entity.
        /// </summary>
        public ET Entity { get; set; }

    }
}
