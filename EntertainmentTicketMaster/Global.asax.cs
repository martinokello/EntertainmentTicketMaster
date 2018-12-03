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
using EntertainmentTicketMaster.Migrations;

namespace EntertainmentTicketMaster
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public static Cache ApiCache { get; set; }
         protected void Application_Start()
        {
            EntityFrameWorkUpdateV2 frame =new EntityFrameWorkUpdateV2();
            frame.Up();
            ApiCache = HttpRuntime.Cache;
            UnityConfig.RegisterComponents();  
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
        
    }
}
