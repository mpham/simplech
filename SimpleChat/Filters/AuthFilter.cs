using SimpleChat.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SimpleChat.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            AuthenticationHeaderValue authHeader = actionContext.Request.Headers.Authorization;

            var decoded = AuthHelper.DecodeToken(authHeader.Parameter);
            Type type = decoded.GetType();
            if (type == typeof(String))
            {
                throw new HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
            }

            IDictionary<string, object> d = decoded as IDictionary<string, object>;

            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("user_id", d["id"]));
            
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AuthenticationHeaderValue authHeader = actionExecutedContext.Request.Headers.Authorization;
                actionExecutedContext.Response.Headers.Add("Authorization", authHeader.Scheme + " " + authHeader.Parameter);
                actionExecutedContext.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                actionExecutedContext.Response.Content.Headers.ContentType.CharSet = "utf-8";
            }
            
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}