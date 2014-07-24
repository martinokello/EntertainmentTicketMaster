using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using Moq;
using Entertainment.Unit.Tests.UnitOfWork;
using EntertainmentTicketMaster.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoryServices.Services;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.UnitOfWork;
using TicketMasterDataAccess.Dto;

namespace Entertainment.Unit.Tests
{
    [TestClass]
    public class RepositoryTicketServicesTest
    {
        private Address[] addresses;
        private Event[] events;
        private Ticket[] tickets;
        private TicketMasterUser[] users;
        private Booking[] bookings;
        private BookingTicketInfo[] bookingTicketInfos;

        private TicketsController _ticketController;
        private IEventRepositorySegregator _eventRepositorySegregator;
        private ITicketMasterUserRepositorySegregator _ticketMasterUserRepositorySegregator;
        private ITicketRepositorySegregator _ticketRepositorySegregator;
        private IBookingRepositorySegregator _bookingRepositorySegregator;
        private IAddressRepositorySegregator _addressRepositorySegregator;

        private IRepositoryTicketServiceSegregator _repositoryTicketServiceSegregator;
        private IRepositoryAdminServiceSegregator _repositoryAdminServiceSegregator;

        [TestInitialize]
        public void SetUp()
        {
            users = new TicketMasterUser[] { new TicketMasterUser { UserId = 1, Email = "martin.okello@gmail.com", UserName = "martinokello" }, new TicketMasterUser { UserId = 2, Email = "joanne.okello@gmail.com", UserName = "joanneokello" } };
            addresses = new Address[] { new Address { AddressId = 1, AddressLine1 = "amb1", AddressLine2 = "add2", Country = "UK", PostCode = "SW2 3EB", TicketMasterUser = users[0], UserId = 1, Town = "London" }, new Address { AddressId = 2, AddressLine1 = "eve", AddressLine2 = "addeve2", Country = "Uganda", PostCode = "P.O.Box 32", TicketMasterUser = users[1], UserId = 2, Town = "Kampala" } };
            events = new Event[]{new Event{EventDate = DateTime.Now,EventDescription = "EventA",EventId = 1,EventName = "EventA",Location = "London Olympia",NumberOfTickets = 120},new Event{EventDate = DateTime.Now,EventDescription = "EventB",EventId = 2,EventName = "EventB",Location = "State House Kampala",NumberOfTickets = 49}};
            tickets = new Ticket[] { new Ticket { EventId = 1, Price = (decimal)3.89, TicketGUID = Guid.NewGuid(), TicketId = 1, Event = events[0] }, new Ticket { EventId = 2, Price = (decimal)3.59, TicketGUID = Guid.NewGuid(), TicketId = 2, Event = events[1] } };
            bookings = new Booking[] { new Booking { BookingDate = DateTime.Now, BookingId = 1, IsVerifiedPayment = false, NumberOfTickets = 4, Ticket = tickets[0] }, new Booking { BookingDate = DateTime.Now, BookingId = 2, IsVerifiedPayment = false, NumberOfTickets = 8, Ticket = tickets[1] } };
            bookingTicketInfos = new BookingTicketInfo[]{ new BookingTicketInfo{ },new BookingTicketInfo{ }};

            var ticketRepository= new Mock<TicketRepository>(new FakeUnitOfWork());
            var eventRepository = new Mock<EventRepository>(new FakeUnitOfWork());
            var ticketMasterUserRepository = new Mock<TicketMasterUserRepository>(new FakeUnitOfWork());
            var bookingRepository =  new Mock<BookingRepository>(new FakeUnitOfWork());
            var addressRepository = new Mock<AddressRepository>(new FakeUnitOfWork());

            ticketRepository.Setup(p => p.GetById(It.IsAny<int>())).Returns(tickets[0]);
            ticketRepository.Setup(p => p.Add(It.IsAny<Ticket>())).Returns(true);
            ticketRepository.Setup(p => p.Delete(It.IsAny<int>())).Returns(true);
            ticketRepository.Setup(p => p.Update(It.IsAny<Ticket>())).Returns(true);
            ticketRepository.Setup(p => p.GetAll()).Returns(tickets);

            eventRepository.Setup(p => p.GetById(It.IsAny<int>())).Returns(events[0]);
            eventRepository.Setup(p => p.Add(It.IsAny<Event>())).Returns(true);
            eventRepository.Setup(p => p.Delete(It.IsAny<int>())).Returns(true);
            eventRepository.Setup(p => p.Update(It.IsAny<Event>())).Returns(true);
            eventRepository.Setup(p => p.GetAll()).Returns(events);

            bookingRepository.Setup(p => p.GetById(It.IsAny<int>())).Returns(bookings[0]);
            bookingRepository.Setup(p => p.Add(It.IsAny<Booking>())).Returns(true);
            bookingRepository.Setup(p => p.Delete(It.IsAny<int>())).Returns(true);
            bookingRepository.Setup(p => p.Update(It.IsAny<Booking>())).Returns(true);
            bookingRepository.Setup(p => p.GetAll()).Returns(bookings);
            bookingRepository.Setup(p => p.GetTicketsForUser(It.IsAny<string>())).Returns(bookings);
            bookingRepository.Setup(p => p.GetTicketsForUserVerified()).Returns(new BookingTicketInfo[]{new BookingTicketInfo{BookingId = 1,EventName="EventA",IsVerifiedPayment = true,NumberOfTickets = 5,Username = "martinokello"}});
            bookingRepository.Setup(p => p.GetBookingsByEvent(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new GroupedBooking[] { new GroupedBooking() });

            addressRepository.Setup(p => p.GetById(It.IsAny<int>())).Returns(addresses[0]);
            addressRepository.Setup(p => p.GetAddressByUsername(It.IsAny<string>())).Returns(addresses[0]);
            addressRepository.Setup(p => p.Add(It.IsAny<Address>())).Returns(true);
            addressRepository.Setup(p => p.Delete(It.IsAny<int>())).Returns(true);
            addressRepository.Setup(p => p.Update(It.IsAny<Address>())).Returns(true);
            addressRepository.Setup(p => p.GetAll()).Returns(addresses);

            ticketMasterUserRepository.Setup(p => p.GetById(It.IsAny<int>())).Returns(users[0]);
            ticketMasterUserRepository.Setup(p => p.GetUserByName(It.IsAny<string>())).Returns(users[0]);
            ticketMasterUserRepository.Setup(p => p.Add(It.IsAny<TicketMasterUser>())).Returns(true);
            ticketMasterUserRepository.Setup(p => p.Delete(It.IsAny<int>())).Returns(true);
            ticketMasterUserRepository.Setup(p => p.Update(It.IsAny<TicketMasterUser>())).Returns(true);
            ticketMasterUserRepository.Setup(p => p.GetAll()).Returns(users);

            var ticketService = new Mock<RepositoryTicketServices>(ticketRepository, eventRepository,
                ticketMasterUserRepository, bookingRepository);
            ticketService.Setup(p => p.GetTicketForUser(It.IsAny<string>())).Returns(tickets);
            var adminService = new Mock<RepositoryAdminServices>(ticketRepository, eventRepository,
                ticketMasterUserRepository, bookingRepository);
            adminService.Setup(p => p.GetVerifiedBooking()).Returns(bookingTicketInfos);
            
            _repositoryTicketServiceSegregator =  new RepositoryTicketServices(ticketRepository.Object,eventRepository.Object,ticketMasterUserRepository.Object,bookingRepository.Object,addressRepository.Object);
            _repositoryAdminServiceSegregator = new RepositoryAdminServices(ticketRepository.Object,eventRepository.Object, ticketMasterUserRepository.Object, bookingRepository.Object);

            _ticketController = new TicketsController(_repositoryTicketServiceSegregator,_repositoryAdminServiceSegregator);
        }

        [TestMethod]
        public void Repository_Ticket_Service_Test_Calls_Ticket_For_User_Returns_The_Right_Result()
        {
            var result = (_repositoryTicketServiceSegregator as RepositoryTicketServices).GetTicketForUser("martinokello");

            Assert.AreEqual(result.Length, 2);
        }


        [TestMethod]
        public void Repository_Ticket_Service_Test_Calls_GetEventById_Returns_The_Right_Result()
        {
            var result = (_repositoryTicketServiceSegregator as RepositoryTicketServices).GetEventById(1);
            Assert.AreEqual(result.EventName, "EventA");
        }

        [TestMethod]
        public void Repository_Ticket_Service_Test_Calls_GetAddressByUsername_Returns_The_Right_Result()
        {
            var result = (_repositoryTicketServiceSegregator as RepositoryTicketServices).GetAddressByUsername("martinokello");
            Assert.AreEqual(result.AddressLine1, "amb1");
        }

        [TestMethod]
        public void Repository_Ticket_Service_Test_Calls_GetAllEvents_Returns_The_Right_Result()
        {
            var result = (_repositoryTicketServiceSegregator as RepositoryTicketServices).GetAllEvents();
            Assert.AreEqual(result.Length, 2);
        }

        [TestMethod]
        public void Repository_Ticket_Service_Test_Calls_AddNewClientAddress_Returns_The_Right_Result()
        {
            var result = (_repositoryTicketServiceSegregator as RepositoryTicketServices).AddNewClientAddress("martinalex",new Address{AddressId = 4,AddressLine1 = "xy1",AddressLine2 = "xy2",Country = "UK",PostCode = "xys 3ab",TicketMasterUser = new TicketMasterUser(),Town="bugansa",UserId = 5});
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Repository_Ticket_Serice_Test_Call_GetBookingsByEvent_Returns_Right_Result()
        {
            var fro = DateTime.Now.AddDays(-3);
            var to = DateTime.Now;

            var result = (_repositoryTicketServiceSegregator as RepositoryTicketServices).GetBookingsByEvent(fro, to);
            Assert.IsNotNull(result);
        }
    }
}
