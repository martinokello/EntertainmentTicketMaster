using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EntertainmentTicketMaster.App_Start;

namespace EntertainmentTicketMaster
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public static Cache ApiCache { get; set; }
        protected void Application_Start()
        {
            ApiCache = HttpRuntime.Cache;
            UnityConfig.RegisterTypes(UnityConfig.GetConfiguredContainer());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
