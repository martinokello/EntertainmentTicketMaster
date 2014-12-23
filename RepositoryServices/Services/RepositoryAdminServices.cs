using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Dto;
using TicketMasterDataAccess.UnitOfWork;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace RepositoryServices.Services
{
    public class RepositoryAdminServices : IRepositoryAdminServiceSegregator
    {        
        private TicketRepository _ticketRepository;
        private TicketMasterUserRepository _ticketMasterUserRepository;
        private EventRepository _eventRepository;
        private BookingRepository _bookingRepository;

        public RepositoryAdminServices()
        {
            _ticketMasterUserRepository = new TicketMasterUserRepository(new UnitOfWork());
     
        }
        public RepositoryAdminServices(ITicketRepositorySegregator ticketRepository,IEventRepositorySegregator eventRepository, ITicketMasterUserRepositorySegregator ticketMasterUserRepository,IBookingRepositorySegregator bookingRepository)
        {
            _ticketRepository = ticketRepository as TicketRepository;
            _eventRepository = eventRepository as EventRepository;
            _ticketMasterUserRepository = ticketMasterUserRepository as TicketMasterUserRepository;
            _bookingRepository = bookingRepository as BookingRepository;
        }

        public bool AddEvent(Event evnt)
        {
            return _eventRepository.Add(evnt);
        }

        public bool AddUser(TicketMasterUser user)
        {
            return _ticketMasterUserRepository.Add(user);
        }
        public bool AddTicket(Ticket ticket)
        {
            return _ticketRepository.Add(ticket);
        }

        public Event[] GetAllEvents()
        {
            return _eventRepository.GetAll();
        }

        public Ticket[] GetAllTickets()
        {
            return _ticketRepository.GetAll();
        }

        public TicketMasterUser[] GetAllUsers()
        {
            return _ticketMasterUserRepository.GetAll();
        }

        public bool UpdateEvent(Event evnt)
        {
            return _eventRepository.Update(evnt);
        }

        public virtual BookingTicketInfo[] GetVerifiedBooking()
        {
            return _bookingRepository.GetTicketsForUserVerified();
        }

        public virtual BookingStats[] GetStatsByMonth(DateTime startDate, DateTime endDate)
        {
            return _bookingRepository.GetStatsByMonth(startDate, endDate);
        }
    }

    public interface IRepositoryAdminServiceSegregator
    {
        bool AddEvent(Event evnt);

        bool AddUser(TicketMasterUser user);

        bool AddTicket(Ticket ticket);

        Event[] GetAllEvents();

        Ticket[] GetAllTickets();

        TicketMasterUser[] GetAllUsers();

        bool UpdateEvent(Event evnt);

        BookingTicketInfo[] GetVerifiedBooking();
    }
}
