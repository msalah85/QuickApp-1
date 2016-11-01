﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace QuickApp.AspNetCore.Mvc
{
    public static class RouteBuilderExtension
    {
        public static void AddQuickAppRoute(this IRouteBuilder routeBuilder, string urlPrefix = null)
        {
            if (urlPrefix == null)
                urlPrefix = "";
            else if (!urlPrefix.EndsWith("/"))
                urlPrefix += "/";

            routeBuilder.MapRoute(
                    name: "quickapp",
                    template: urlPrefix + "{serviceName}/{methodName}",
                    defaults: new { controller = "QuickApp", action = "CallServiceMethod" });
        }
    }
}