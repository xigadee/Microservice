using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class SearchExpressionHelper<E>
    {
        PropertyMap mPropertyMap;

        public SearchExpressionHelper()
        {
            mPropertyMap = GetPropertyMap();
        }

        /// <summary>
        /// This is root property map.
        /// </summary>
        protected class PropertyMap
        {
            public PropertyMap(Type rootType)
            {

            }

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

        public Expression Build(SearchRequest key)
        {
            return null;
        }
    }
}
