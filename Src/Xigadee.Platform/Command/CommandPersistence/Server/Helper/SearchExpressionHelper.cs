using System;
using System.Linq;
using System.Linq.Expressions;

namespace Xigadee
{
    /// <summary>
    /// This class is used to build a search expression. It's not currently used.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class SearchExpressionHelper<E>
    {
        PropertyMap mPropertyMap;
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchExpressionHelper{E}"/> class.
        /// </summary>
        public SearchExpressionHelper()
        {
            mPropertyMap = GetPropertyMap();
        }

        /// <summary>
        /// This is root property map.
        /// </summary>
        protected class PropertyMap
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyMap"/> class.
            /// </summary>
            /// <param name="rootType">Type of the root.</param>
            public PropertyMap(Type rootType)
            {
                RootType = rootType;
            }
            /// <summary>
            /// Gets the entity type.
            /// </summary>
            public Type RootType { get; }
        }

        private static PropertyMap GetPropertyMap(Type type = null)
        {
            if (type == null)
                type = typeof(E);

            var map = new PropertyMap(type);

            var properties = type.GetProperties().Where(p => p.CanRead);

            return map;
        }
        /// <summary>
        /// Builds the specified expression base on the search request..
        /// </summary>
        /// <param name="key">The search request..</param>
        /// <returns>Returns the expression.</returns>
        public Expression Build(SearchRequest key)
        {
            return null;
        }
    }
}
