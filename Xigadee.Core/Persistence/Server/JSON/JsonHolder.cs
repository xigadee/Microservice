#region using

#endregion
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
