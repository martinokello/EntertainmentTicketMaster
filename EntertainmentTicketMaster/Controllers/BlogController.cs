using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EntertainmentTicketMaster.Controllers
{
    public class BlogsController : Controller
    {
        //
        // GET: /Blog/
        public ActionResult Blog()
        {
            ViewBag.Title = "Blog";
            return View("Blog");
        }
	}
}