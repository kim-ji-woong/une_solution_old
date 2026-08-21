using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SOPWebAPI
{
    public static class WebApiConfig
    {
        public const string DEFAULT_ROUTE_NAME = "DefaultApi";

        public static void Register(HttpConfiguration config)
        {
            // Web API 구성 및 서비스
            
            // Web API 경로
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: DEFAULT_ROUTE_NAME,
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
