using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using EntertainmentTicketMaster.Models;
using Microsoft.Owin.Security.DataHandler.Encoder;
using RepositoryServices.Services;
using TicketMasterDataAccess.DataAccess;
using UPAEventsPayPal;

namespace EntertainmentTicketMaster.Controllers
{
    public class TicketsController : Controller
    {
        private RepositoryTicketServices _repositoryTicketServices;
        private RepositoryAdminServices _repositoryAdminServices;

        public TicketsController(IRepositoryTicketServiceSegregator repositoryTicketServices, IRepositoryAdminServiceSegregator repositoryAdminServices)
        {
            _repositoryTicketServices = repositoryTicketServices as RepositoryTicketServices;
            _repositoryAdminServices = repositoryAdminServices as RepositoryAdminServices;
        }
        //
        // GET: /Ticket/
        public ActionResult TicketsInfo()
        {
            ViewBag.Title = "Ticket Info";
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                var tickets =
                    _repositoryTicketServices.GetTicketForUser(User.Identity.Name);
                return View("TicketInfo", tickets);
            }
            return View("TicketInfo");
        }

        [HttpGet]
        public ActionResult EventInfo(int id)
        {
            ViewBag.Title = "Event Info";
            var evnt = _repositoryTicketServices.GetEventById(id);

            return View("EventInfo", new EventViewModel { EventId = evnt.EventId, EventName = evnt.EventName, EventDescription = evnt.EventDescription, EventDate = ((DateTime)evnt.EventDate).ToString("dd/MM/yyyy HH:mm") });
        }

        [HttpGet]
        public JsonResult GetUserAddress()
        {
            var address = _repositoryTicketServices.GetAddressByUsername(User.Identity.Name);
            var model = new AddressViewModel
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Town = address.Town,
                PostCode = address.PostCode,
                Country = address.Country
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EventComingSoon()
        {
            ViewBag.Title = "Events";

            var events =
                _repositoryTicketServices.GetAllEvents()
                    .Where(p => p.EventDate >= DateTime.Now.Add(new TimeSpan(14,0,0,0)));

            var viewModel = from e in events
                            select new EventViewModel
                            {
                                EventId = e.EventId,
                                EventName = e.EventName,
                                EventDate = ((DateTime)e.EventDate).ToString("dd/MM/yyyy HH:mm", new CultureInfo("en-GB")),
                                Location = e.Location
                            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize]
        [ChildActionOnly]
        public PartialViewResult ClientAddress()
        {
            var address = _repositoryTicketServices.GetAddressByUsername(User.Identity.Name);
            if (address == null)
            {
                address = new Address();
            }
            var model = new AddressViewModel
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Town = address.Town,
                PostCode = address.PostCode,
                Country = address.Country
            };
            return PartialView("ClientAddress",model);
        }


        [HttpPost]
        [Authorize]
        public PartialViewResult ClientAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingAddress = _repositoryTicketServices.GetAddressByUsername(User.Identity.Name);

                if (existingAddress != null)
                {
                    existingAddress.AddressLine1 = model.AddressLine1;
                    existingAddress.AddressLine2 = model.AddressLine2;
                    existingAddress.Town = model.Town;
                    existingAddress.Country = model.Country;
                    existingAddress.PostCode = model.PostCode;

                    _repositoryTicketServices.UpdateAddressByUsername(User.Identity.Name,existingAddress);
                }
                else
                {
                    var newAddress = new Address
                    {
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        Town = model.Town,
                        PostCode = model.PostCode,
                        Country = model.Country
                    };

                    _repositoryTicketServices.AddNewClientAddress(User.Identity.Name,newAddress);
                }
                return PartialView("_PatialSuccess");
            }
            return PartialView(model);
        }
        [Authorize]
        public ActionResult BookTickets()
        {
            ViewBag.Title = "Book Tickets";
            var events = _repositoryTicketServices.GetAllEvents();

            var eventList = new SelectListItem[events.Length + 1];

            var index = 0;
            eventList[index] = new SelectListItem { Selected = false, Text = "Select Event", Value = (-1).ToString() };

            foreach (var evt in events)
            {
                eventList[index + 1] = new SelectListItem { Selected = false, Text = evt.EventName, Value = evt.EventId.ToString() };
                index++;
            }
            ViewBag.Events = eventList;

            return View("BookTickets");
        }
        [HttpPost]
        [Authorize]
        public ActionResult BookTickets(TicketViewModel model)
        {
            ViewBag.Title = "Book Tickets";
            var events = _repositoryTicketServices.GetAllEvents();
            var eventList = new SelectListItem[events.Length + 1];

            var index = 0;
            eventList[index] = new SelectListItem
            {
                Selected = false,
                Text = "Select Event",
                Value = (-1).ToString()
            };

            foreach (var evt in events)
            {
                eventList[index + 1] = new SelectListItem
                {
                    Selected = false,
                    Text = evt.EventName,
                    Value = evt.EventId.ToString()
                };
                index++;
            }
            ViewBag.Events = eventList;

            int eventId = -1;
            if (!int.TryParse(model.EventName, out eventId) || model.NumberOfTickets < 1 || model.Price <= (decimal)0.0 || model.TotalPrice <= (decimal)0.00)
            {
                ModelState.AddModelError("transactionVoid", "Price, Number of tickets required");
            }
            try
            {

                if (ModelState.IsValid)
                {
                    eventId = -1;
                    if (int.TryParse(model.EventName, out eventId))
                    {
                        var evnt = _repositoryTicketServices.GetEventById(eventId);
                        var user = _repositoryTicketServices.GetUserByName(User.Identity.Name);

                        var ticket = new Ticket
                        {
                            EventId = eventId,
                            Price = model.Price,
                            TicketGUID = Guid.NewGuid()
                        };

                        var bookingId = _repositoryTicketServices.BookTickets(ticket, model.NumberOfTickets, user.UserId);
                        var paypalBaseUrl = ConfigurationManager.AppSettings["PaypalBaseUrl"];
                        var cancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
                        var successUrl = ConfigurationManager.AppSettings["SuccessUrl"];
                        var notifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
                        var smtpServer = ConfigurationManager.AppSettings["SmtpHostServer"];
                        var businessEmail = ConfigurationManager.AppSettings["BusinessEmail"];
                        var customer = _repositoryTicketServices.GetUserByName(User.Identity.Name);

                        var buyerEmail = customer.Email;
                        var product = new Product
                        {
                            Ammount = model.Price,
                            ProductDescription = evnt.EventDescription,
                            ProductName = evnt.EventName,
                            Quantity = model.NumberOfTickets,
                            VATAmmount = 0
                        };
                        var products = new List<Product> { product };
                        Session["ShoppingBasket"] = products;
                        var upaProducts = products;
                        var totalAmount = model.TotalPrice;

                        Session["InvoiceNo"] = bookingId;
                        Session["ProductsUPA"] = upaProducts;
                        Session["buyerEmail"] = buyerEmail;

                        var context = HttpContext;
                        //Process Payment
                        var paypal = new PayPalHandler(context.ApplicationInstance.Context.Session, System.Configuration.ConfigurationManager.AppSettings["PaypalBaseUrl"],
                            System.Configuration.ConfigurationManager.AppSettings["BusinessEmail"],
                            System.Configuration.ConfigurationManager.AppSettings["SuccessUrl"],
                            System.Configuration.ConfigurationManager.AppSettings["CancelUrl"],
                            System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"]);

                        paypal.Response = context.ApplicationInstance.Context.Response;


                        paypal.RedirectToPayPal();
                        return View("BookedSuccess");
                    }

                }
            }
            catch (Exception e)
            {

            }
            return View("BookTickets", model);
        }

        public ActionResult Events()
        {
            ViewBag.Title = "Events";

            var events =
                _repositoryTicketServices.GetAllEvents()
                    .Where(p => p.EventDate >= DateTime.Now);

            var viewModel = from e in events
                            select new EventViewModel
                            {
                                EventId = e.EventId,
                                EventName = e.EventName,
                                EventDate = ((DateTime) e.EventDate).ToString("dd/MM/yyyy HH:mm",new CultureInfo("en-GB")),
                                Location = e.Location
                            };

            return View("Events", viewModel);
        }

        
        public JsonResult GetTicketUnitPriceByEventId(int eventId)
        {
            var evnt = _repositoryTicketServices.GetEventById(eventId);
            var eventViewModel = new EventViewModel
            {
                EventId = evnt.EventId,
                NumberOfTickets = evnt.NumberOfTickets.Value,
                Price = evnt.PricePerTicket.Value,
                EventName = evnt.EventName
            };
            return Json(eventViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}