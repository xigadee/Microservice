using System.Text;
namespace Xigadee
{
    /// <summary>
    /// This is the root entity holder.
    /// </summary>
    public class JsonHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHolder"/> class.
        /// </summary>
        /// <param name="Version">The version.</param>
        /// <param name="Json">The entity as a json string.</param>
        /// <param name="Id">The identifier.</param>
        public JsonHolder(string Version, string Json, string Id = null)
        {
            this.Version = Version;
            this.Json = Json;
            this.Id = Id;
        }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Gets or sets the json.
        /// </summary>
        public string Json { get; set; }
        /// <summary>
        /// Converts the JSON to a binary array.
        /// </summary>
        /// <returns>Returns a UTF8 representation on the JSON string.</returns>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHolder{KT}"/> class.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Version">The version.</param>
        /// <param name="Json">The entity as a json string.</param>
        /// <param name="Id">The identifier.</param>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHolder{KT, ET}"/> class.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <param name="Key">The entity key.</param>
        /// <param name="Version">The entity version.</param>
        /// <param name="Json">The entity as a json string.</param>
        /// <param name="Id">The entity identifier as a string.</param>
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
