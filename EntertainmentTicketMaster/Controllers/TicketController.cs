using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntertainmentTicketMaster.Models;
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
            return View("TicketInfo", null);
        }

        [HttpGet]
        public ActionResult EventInfo(int id)
        {
            ViewBag.Title = "Event Info";
            var evnt = _repositoryTicketServices.GetEventById(id);

            return View("EventInfo", new EventViewModel { EventId = evnt.EventId, EventName = evnt.EventName, Location = evnt.Location, EventDescription = evnt.EventDescription, EventDate = ((DateTime)evnt.EventDate).ToString("dd/MM/yyyy HH:mm") });
        }

        [HttpGet]
        public JsonResult GetUserEntertainmentAddress()
        {
            var EntertainmentAddress = _repositoryTicketServices.GetEntertainmentAddressByUsername(User.Identity.Name);
            var model = new EntertainmentAddressViewModel
            {
                AddressLine1 = EntertainmentAddress.AddressLine1,
                AddressLine2 = EntertainmentAddress.AddressLine2,
                Town = EntertainmentAddress.Town,
                PostCode = EntertainmentAddress.PostCode,
                Country = EntertainmentAddress.Country
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EventComingSoon()
        {
            ViewBag.Title = "Events";

            var events =
                _repositoryTicketServices.GetAllEvents()
                    .Where(p => p.EventDate >= DateTime.Now.Add(new TimeSpan(14, 0, 0, 0)));

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
            var EntertainmentAddress = _repositoryTicketServices.GetEntertainmentAddressByUsername(User.Identity.Name);
            if (EntertainmentAddress == null)
            {
                EntertainmentAddress = new EntertainmentAddress();
            }
            var model = new EntertainmentAddressViewModel
            {
                AddressLine1 = EntertainmentAddress.AddressLine1,
                AddressLine2 = EntertainmentAddress.AddressLine2,
                Town = EntertainmentAddress.Town,
                PostCode = EntertainmentAddress.PostCode,
                Country = EntertainmentAddress.Country
            };
            return PartialView("ClientAddress", model);
        }


        [HttpPost]
        [Authorize]
        public PartialViewResult ClientAddress(EntertainmentAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingEntertainmentAddress = _repositoryTicketServices.GetEntertainmentAddressByUsername(User.Identity.Name);

                if (existingEntertainmentAddress != null)
                {
                    existingEntertainmentAddress.AddressLine1 = model.AddressLine1;
                    existingEntertainmentAddress.AddressLine2 = model.AddressLine2;
                    existingEntertainmentAddress.Town = model.Town;
                    existingEntertainmentAddress.Country = model.Country;
                    existingEntertainmentAddress.PostCode = model.PostCode;

                    _repositoryTicketServices.UpdateEntertainmentAddressByUsername(User.Identity.Name, existingEntertainmentAddress);
                }
                else
                {
                    var newEntertainmentAddress = new EntertainmentAddress
                    {
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        Town = model.Town,
                        PostCode = model.PostCode,
                        Country = model.Country
                    };

                    _repositoryTicketServices.AddNewClientAddress(User.Identity.Name, newEntertainmentAddress);
                }
                return PartialView("_PatialSuccess");
            }
            return PartialView(model);
        }
        [Authorize]
        public ActionResult BookTickets()
        {
            ViewBag.Title = "Book Tickets";
            PopulateUIEventsView();

            return View("BookTickets");
        }

        private void PopulateUIEventsView()
        {
            var events = _repositoryTicketServices.GetAllCurrentEvents();
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
        }
        [HttpPost]
        [Authorize]
        public ActionResult BookTickets(TicketViewModel model)
        {
            ViewBag.Title = "Book Tickets";
            PopulateUIEventsView();

            int eventId = -1;
            if (!int.TryParse(model.EventName, out eventId) || model.NumberOfTickets < 1 || model.Price <= (decimal)0.0 || model.TotalPrice <= (decimal)0.00)
            {
                ModelState.AddModelError("transactionVoid", "Price, Number of tickets required");
            }
            try
            {

                if (ModelState.IsValid)
                {
                    var evnt = _repositoryTicketServices.GetEventById(eventId);
                    var user = _repositoryTicketServices.GetUserByName(User.Identity.Name);
                    if (evnt == null || eventId == -1)
                    {
                        ModelState.AddModelError("chooseEvent", "Event needs to be chosen");
                        return View("BookTickets", model);
                    }
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

                    Session["InvoiceNo"] = bookingId;
                    Session["ProductsUPA"] = upaProducts;
                    Session["buyerEmail"] = buyerEmail;

                    var context = HttpContext;
                    //Process Payment
                    var paypal = new PayPalHandler(context.ApplicationInstance.Context.Session,
                        paypalBaseUrl, businessEmail, successUrl, cancelUrl, notifyUrl);

                    paypal.Response = context.ApplicationInstance.Context.Response;


                    paypal.RedirectToPayPal();
                    return View("BookedSuccess");
                }

                else return View(model);
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
                                EventDate = ((DateTime)e.EventDate).ToString("dd/MM/yyyy HH:mm", new CultureInfo("en-GB")),
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