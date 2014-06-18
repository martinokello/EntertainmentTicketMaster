using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RepositoryServices.Services;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.UnitOfWork;
using UPAEventsPayPal;

namespace EntertainmentTicketMaster.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            ViewBag.Title = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contact";
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PaypalNotify(FormCollection formCollection)
        {
            var paymentVerification = new InstantPaymentNotification(HttpContext.ApplicationInstance.Context.Request,
                ConfigurationManager.AppSettings["BusinessEmail"], formCollection, new BookingRepository(new UnitOfWork()));

            FileInfo fileInfo = new FileInfo(Server.MapPath("~/IPN_Notification/IPNMessage.txt"));
            var ipnWriter = fileInfo.CreateText();

            var isVerified = paymentVerification.ProcessIPNResults(HttpContext.ApplicationInstance.Context, ipnWriter);
            if (isVerified)
                return Content("IPN Verification Successfull");
            return Content("IPN Verification Failed");
        }
    }
}