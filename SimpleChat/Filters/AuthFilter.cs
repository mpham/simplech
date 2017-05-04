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

            IDictionary<string, object> decoded = AuthHelper.DecodeToken(authHeader.Parameter);
            object errorMessage;
            if (decoded.TryGetValue("error", out errorMessage))
            {
                throw new HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
            }

            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("user_id", decoded["id"]));
            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("token", authHeader.Parameter));

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