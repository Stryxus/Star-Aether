using System;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SA.Web.Server.Data.Json;

namespace SA.Web.Server.Data
{
    public class AdminIPFiltered : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            IPAddress connectingIP = context.HttpContext.Connection.RemoteIpAddress;
            IPAddress adminIP = IPAddress.Parse(PrivateVariables.GetAdminIP());

            if (connectingIP.IsIPv4MappedToIPv6) connectingIP = connectingIP.MapToIPv4();

            if (!adminIP.Equals(connectingIP))
            {
                await Logger.LogWarn("Unauthorized IP connected to admin controller: " + connectingIP);
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}