using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;

namespace SOPWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Models.DataManager.LoadData();
            //Models.DataManager.WriteLog("Application_Start");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            //SOPWebAPI.WebApiConfig.Register(GlobalConfiguration.Configuration);
            //SOPWebAPI.FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected void Application_End()
        {
            // IIS 재시작 혹은 중단시 호출
            //Models.DataManager.WriteLog("Application_End");
        }
    }
}
