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
        private IUnitOfWork _ticketUnitOfWork;
        private IUnitOfWork _eventUnitOfWork;
        private IUnitOfWork _ticketUserUnitOfWork;
        private IUnitOfWork _bookingUnitOfWork;


        public RepositoryAdminServices(ITicketMasterUserRepositorySegregator userRepository)
        {
            var dbContext = new TicketMasterEntities();
            _ticketMasterUserRepository = userRepository as TicketMasterUserRepository;
            _ticketUserUnitOfWork = new UnitOfWork<TicketMasterUser>(_ticketMasterUserRepository);
            _ticketMasterUserRepository.DBContext = (_ticketUserUnitOfWork as UnitOfWork<TicketMasterUser>).DBContext = dbContext;
        }
        public RepositoryAdminServices(ITicketRepositorySegregator ticketRepository,IEventRepositorySegregator eventRepository, ITicketMasterUserRepositorySegregator userRepository,IBookingRepositorySegregator bookingRepository)
        {

            var  dbContext = new TicketMasterEntities();
            _ticketRepository = ticketRepository as TicketRepository;
            _ticketUnitOfWork = new UnitOfWork<Ticket>(_ticketRepository);
            _ticketRepository.DBContext = (_ticketUnitOfWork as UnitOfWork<Ticket>).DBContext = dbContext;
            _eventRepository = eventRepository as EventRepository;
            _eventUnitOfWork = new UnitOfWork<Event>(_eventRepository);
            _eventRepository.DBContext = (_eventUnitOfWork as UnitOfWork<Event>).DBContext = dbContext;
            _ticketMasterUserRepository = userRepository as TicketMasterUserRepository;
            _ticketUserUnitOfWork = new UnitOfWork<TicketMasterUser>(_ticketMasterUserRepository);
            _ticketMasterUserRepository.DBContext = (_ticketUserUnitOfWork as UnitOfWork<TicketMasterUser>).DBContext = dbContext;
            _bookingRepository = bookingRepository as BookingRepository;
            _bookingUnitOfWork = new UnitOfWork<Booking>(_bookingRepository);
            _bookingRepository.DBContext = (_bookingUnitOfWork as UnitOfWork<Booking>).DBContext = dbContext;
        }

        public bool AddEvent(Event evnt)
        {
            var result = _eventRepository.Add(evnt);
            _eventUnitOfWork.SaveChanges();
            return result;
        }

        public bool AddUser(TicketMasterUser user)
        {
            var result = _ticketMasterUserRepository.Add(user);
            _ticketUserUnitOfWork.SaveChanges();
            return result;
        }
        public bool AddTicket(Ticket ticket)
        {
            var result = _ticketRepository.Add(ticket);
            _ticketUnitOfWork.SaveChanges();
            return result;
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
            var result = _eventRepository.Update(evnt);
            _eventUnitOfWork.SaveChanges();
            return result;
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
