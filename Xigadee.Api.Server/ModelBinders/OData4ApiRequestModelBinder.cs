
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Xigadee
{
    public class OData4ApiRequestModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            //TODO: See http://www.asp.net/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api
            if (bindingContext.ModelType != typeof(OData4ApiRequest))
            {
                return false;
            }

            OData4ApiRequest result = new OData4ApiRequest
            {
                Options = ApiUtility.BuildRepositorySettings(actionContext.RequestContext, actionContext.Request)
            };

            bindingContext.Model = result;

            if (actionContext.Request.Headers.Accept!=null)
                result.Accept = actionContext.Request.Headers.Accept.ToList();
            
            if (actionContext.Request.Content.Headers.ContentLength > 0)
            {
                result.BodyType = actionContext.Request.Content.Headers.ContentType.ToString();
                result.Body = actionContext.Request.Content.ReadAsByteArrayAsync().Result;

                return true;
            }

            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null)
                result.Auth = string.Format("{0}|{1}", auth.Scheme, auth.Parameter);
        
            if (bindingContext.ValueProvider.ContainsPrefix("id"))
            {
                result.HasKey = true;
                result.Id = bindingContext.ValueProvider.GetValue("id").RawValue as string;                
                return true;
            }
            
            if (bindingContext.ValueProvider.ContainsPrefix("RefType"))
            {
                result.HasReference = true;
                result.RefType = bindingContext.ValueProvider.GetValue("RefType").RawValue as string;
                result.RefValue = bindingContext.ValueProvider.GetValue("RefValue").RawValue as string;
                return true;
            }

            // If we have reached this point then we haven't been able to bind id or ref type and there is no content
            // We might still have a get all function with no id which we need to cater for e.g. ReadAllGenders. In this
            // instance set the result to has key with a null key and let the back end deal with it
            result.HasKey = true;
            return true;

            //bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert value to Location");
            //return false;
        }
    }
}
