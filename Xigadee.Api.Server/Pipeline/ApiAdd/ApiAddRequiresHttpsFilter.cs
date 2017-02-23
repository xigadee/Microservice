using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        /// <summary>
        /// Adds a requires https filter to the pipeline - prevents non https traffic accessing the api
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="webpipe"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static P ApiAddRequiresHttps<P>(this P webpipe, Action<IAuthenticationFilter> action = null) where P : IPipelineWebApi
        {
            var filter = new RequireHttpsFilter();
            action?.Invoke(filter);
            webpipe.HttpConfig.Filters.Add(filter);
            return webpipe;
        }
    }
}
