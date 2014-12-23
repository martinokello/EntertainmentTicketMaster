using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using EmailServices;
using EmailServices.EmailDomain;
using EmailServices.Interfaces;
using EntertainmentTicketMaster.Models;
using RepositoryServices.Services;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace EntertainmentTicketMaster.Controllers
{
    [Authorize(Users = "administrator")]
    public class AdministrationController : Controller
    {
        private RepositoryTicketServices _repositoryTicketServices;
        private RepositoryAdminServices _repositoryAdminServices;
        private IEmailService _emailService;

        public AdministrationController(IRepositoryTicketServiceSegregator repositoryTicketServices, IRepositoryAdminServiceSegregator repositoryAdminService)
        {
            _repositoryTicketServices = repositoryTicketServices as RepositoryTicketServices;
            _repositoryAdminServices = repositoryAdminService as RepositoryAdminServices;


            var smtpHostServer = System.Configuration.ConfigurationManager.AppSettings["SmtpHostServer"];
            _emailService = new EmailService(smtpHostServer);
        }

        public ActionResult Index()
        {
            var events = _repositoryTicketServices.GetAllEvents();
            ViewBag.Events = events;
            return View();
        }
        //
        // GET: /Administration/
        [HttpGet]
        public ActionResult AddEvent()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetStats(string startDate, string endDate)
        {
            try
            {
                var from = DateTime.Parse(startDate, new CultureInfo("en-GB"));
                var to = DateTime.Parse(endDate, new CultureInfo("en-GB"));
                var model = _repositoryAdminServices.GetStatsByMonth(from, to);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        [HttpPost]
        public ActionResult AddEvent(EventViewModel model, FormCollection formsCollection)
        {
            try
            {
                var time = model.EventTime;
                var regex = new Regex(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$");
                if (!regex.IsMatch(time))
                {
                    ModelState.AddModelError("timeWrong", "the time format should be hh:mm");
                }
                if (ModelState.IsValid)
                {
                    var eventDate = model.EventDate + " " + time;

                    var actualEventDate = DateTime.ParseExact(eventDate, "dd/MM/yyyy HH:mm", new CultureInfo("en-GB"));
                    var attachemntPath = string.Empty;

                    if (model.Attachment != null)
                    {
                        var attachment = model.Attachment;

                        var binaryReader = new BinaryReader(attachment.InputStream);
                        var fileBytes = binaryReader.ReadBytes(attachment.ContentLength);

                        attachemntPath = Server.MapPath("/EventEmailAttachments/" + attachment.FileName);

                        var fileInfo = new FileInfo(attachemntPath);

                        var fileStream = fileInfo.Create();
                        fileStream.Write(fileBytes, 0, fileBytes.Length);
                    }
                    var evnt = new Event
                    {
                        EventDate = actualEventDate,
                        EventName = model.EventName,
                        EventDescription = model.EventDescription,
                        Location = model.Location,
                        PricePerTicket = model.Price,
                        NumberOfTickets = model.NumberOfTickets
                    };

                    if (_repositoryAdminServices.AddEvent(evnt))
                    {
                        return RedirectToAction("Events", "Tickets");
                    }
                    ModelState.AddModelError("eventError", "An error occured. Please inform your administrator!");
                    return View(model);
                }
            }
            catch (Exception e)
            {

            }
            return View(model);
        }

        [HttpGet]

        public ActionResult UpdateEvent(int eventId)
        {
            var evnt = _repositoryTicketServices.GetEventById(eventId);
            var viewModel = new EventViewModel
            {
                EventId = evnt.EventId,
                EventDate = ((DateTime)evnt.EventDate).ToString("dd/MM/yyyy HH:mm"),
                EventDescription = evnt.EventDescription,
                EventName = evnt.EventName,
                NumberOfTickets = evnt.NumberOfTickets.HasValue ? (int)evnt.NumberOfTickets : 0,
                Location = evnt.Location,
                Price = evnt.PricePerTicket.HasValue ? (int)evnt.PricePerTicket.Value : 0
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateEvent(EventViewModel model)
        {
            try
            {
                var time = model.EventTime;
                var regex = new Regex(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$");
                if (!regex.IsMatch(time))
                {
                    ModelState.AddModelError("timeWrong", "the time format should be hh:mm");
                }
                if (ModelState.IsValid)
                {
                    var eventDate = model.EventDate.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0] + " " + time;
                    var actualEventDate = DateTime.ParseExact(eventDate, "dd/MM/yyyy HH:mm", new CultureInfo("en-GB"));
                    var attachemntPath = string.Empty;

                    if (model.Attachment != null)
                    {
                        var attachment = model.Attachment;

                        var binaryReader = new BinaryReader(attachment.InputStream);
                        var fileBytes = binaryReader.ReadBytes(attachment.ContentLength);

                        attachemntPath = Server.MapPath("/EventEmailAttachments/" + attachment.FileName);

                        var fileInfo = new FileInfo(attachemntPath);

                        var fileStream = fileInfo.Create();
                        fileStream.Write(fileBytes, 0, fileBytes.Length);
                    }
                    var evnt = new Event
                    {
                        EventId = model.EventId,
                        EventDate = actualEventDate,
                        EventName = model.EventName,
                        EventDescription = model.EventDescription,
                        Location = model.Location,
                        PricePerTicket = model.Price,
                        NumberOfTickets = model.NumberOfTickets
                    };

                    if (_repositoryAdminServices.UpdateEvent(evnt))
                    {
                        return RedirectToAction("Events", "Tickets");
                    }
                    ModelState.AddModelError("eventError", "An error occured. Please inform your administrator!");
                    return View("UpdateEvent", model);
                }
            }
            catch (Exception e)
            {

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult GetPaidBooking()
        {
            var model = _repositoryAdminServices.GetVerifiedBooking();
            return View(model);
        }
        [HttpGet]
        public ActionResult EmailUsers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmailUsers(BulkEmailContentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var attachemntPath = string.Empty;
                    if (model.Attachment != null)
                    {
                        var attachment = model.Attachment;

                        var binaryReader = new BinaryReader(attachment.InputStream);
                        var fileBytes = binaryReader.ReadBytes(attachment.ContentLength);

                        attachemntPath = Server.MapPath("/EmailAttachments/" + attachment.FileName);

                        var fileInfo = new FileInfo(attachemntPath);

                        var fileStream = fileInfo.Create();
                        fileStream.Write(fileBytes, 0, fileBytes.Length);
                        fileStream.Close();
                    }
                    var users = _repositoryAdminServices.GetAllUsers();
                    var to = new List<string>();
                    to.AddRange(users.Select(e => e.Email));
                    var mailMessage = new TicketMasterEmailMessage
                    {
                        AttachmentFilePath = attachemntPath,
                        EmailFrom = System.Configuration.ConfigurationManager.AppSettings["BusinessEmail"],
                        EmailTo = to,
                        EmailMessage = model.EmailMessage,
                        Subject = "Updates of Events and tickets coming soon"
                    };
                    _emailService.SendEmail(mailMessage);
                    return View("Success");
                }
            }
            catch (Exception e)
            {

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult BookingsStatistics()
        {
            var from = DateTime.Now.AddDays(-365);
            var to = DateTime.Now;
            var model = _repositoryTicketServices.GetBookingsByEvent(from, to);
            return View("BookingsStatistics", model);

        }

        [HttpPost]
        public ActionResult BookingsStatistics(string dateFrom, string dateTo)
        {
            try
            {
                var from = DateTime.Parse(dateFrom, new CultureInfo("en-GB"));
                var to = DateTime.Parse(dateTo, new CultureInfo("en-GB"));
                var model = _repositoryTicketServices.GetBookingsByEvent(from, to);
                return View("BookingsStatistics",model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("datesWrongFormat", "The from and to dates are required and should be of format: dd/MM/yyyy");
                return View("BookingsStatistics");
            }
        }
    }
}