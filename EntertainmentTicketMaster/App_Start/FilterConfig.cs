using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace EntertainmentTicketMaster
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalVariblesSetUp());
        }
    }

    public class GlobalVariblesSetUp:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["SiteRoot"] == null)
            {
                filterContext.HttpContext.Session["SiteRoot"] = ConfigurationManager.AppSettings["SiteRoot2"];
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
