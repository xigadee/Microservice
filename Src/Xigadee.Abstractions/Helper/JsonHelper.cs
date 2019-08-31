using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// This static class provides Json helper methods.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// This static class used the JsonConvert function to deep clone an entity.
        /// </summary>
        /// <typeparam name="C">The class or reference type.</typeparam>
        /// <param name="incoming">The incoming object.</param>
        /// <param name="settings">Any optional Json serialization settings.</param>
        /// <returns>Returns the cloned object.</returns>
        public static C Clone<C>(C incoming, JsonSerializerSettings settings = null) 
        {
            if (object.ReferenceEquals(incoming, default(C)))
                return default(C);

            var json = JsonConvert.SerializeObject(incoming);
            var outObj = JsonConvert.DeserializeObject(json, incoming.GetType());

            return (C)outObj;
        }
    }
}
