#region using

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the JSON data for persistence managers that use it as the base store.
    /// </summary>
    /// <typeparam name="KT">The key type.</typeparam>
    public class JsonHolder<KT>
    {
        public JsonHolder(KT Key, string Version, string Json, string Id = null)
        {
            this.Key = Key;
            this.Version = Version;
            this.Json = Json;
            this.Id = Id;
        }

        public KT Key { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }

        public string Json { get; set; }
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

        public ET Entity { get; set; }

    }
}
