using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Dto;
using TicketMasterDataAccess.Interfaces;
using TicketMasterDataAccess.UnitOfWork;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace RepositoryServices.Services
{
    public class RepositoryTicketServices : IRepositoryTicketServiceSegregator
    {
        private TicketRepository _ticketRepository;
        private TicketMasterUserRepository _ticketMasterUserRepository;
        private EventRepository _eventRepository;
        private BookingRepository _bookingRepository;
        private EntertainmentAddressRepository _entertainmentAddressRepository;

        private IUnitOfWork _ticketUnitOfWork;
        private IUnitOfWork _eventUnitOfWork;
        private IUnitOfWork _ticketUserUnitOfWork;
        private IUnitOfWork _bookingUnitOfWork;
        private IUnitOfWork _entertainmentAddressUnitOfWork;


        public RepositoryTicketServices()
        {
            
        }
        public RepositoryTicketServices(ITicketRepositorySegregator ticketRepository, IEventRepositorySegregator eventRepository, ITicketMasterUserRepositorySegregator userRepository, IBookingRepositorySegregator bookingRepository, IEntertainmentAddressRepositorySegregator entertainmentAddressRepository)
        {
            _ticketRepository = ticketRepository as TicketRepository;
            _ticketUnitOfWork = new UnitOfWork<Ticket>(_ticketRepository);
            _eventRepository = eventRepository as EventRepository;
            _eventUnitOfWork = new UnitOfWork<Event>(_eventRepository);
            _ticketMasterUserRepository = userRepository as TicketMasterUserRepository;
            _ticketUserUnitOfWork = new UnitOfWork<TicketMasterUser>(_ticketMasterUserRepository);
            _bookingRepository = bookingRepository as BookingRepository;
            _bookingUnitOfWork = new UnitOfWork<Booking>(_bookingRepository);
            _entertainmentAddressRepository = entertainmentAddressRepository as EntertainmentAddressRepository;
            _entertainmentAddressUnitOfWork = new UnitOfWork<EntertainmentAddress>(_entertainmentAddressRepository);
        }

        public Event[] GetAllCurrentEvents()
        {
            return _eventRepository.GetAll().Where(p=> p.EventDate != null && (DateTime)p.EventDate >= DateTime.Now).ToArray();
        }
        public Event[] GetAllEvents()
        {
            return _eventRepository.GetAll();
        }
        public Ticket[] GetAllTickets()
        {
            return _ticketRepository.GetAll();
        }

        public virtual Ticket[] GetTicketForUser(string username)
        {
            var bookings = _bookingRepository.GetTicketsForUser(username);

            var tickets = new List<Ticket>();

            foreach (var booking in bookings)
            {
                tickets.Add(booking.Ticket);
            }

            return tickets.ToArray();
        }
        public int BookTickets(Ticket ticket, int numberOfTickets, int userId)
        {
            var result = _bookingRepository.BookTickets(ticket, numberOfTickets, userId);
            _bookingUnitOfWork.SaveChanges();
            return result;
        }

        public Event GetEventById(int eventId)
        {
            return _eventRepository.GetById(eventId);
        }

        public Ticket GetTicketById(int ticketId)
        {
            return _ticketRepository.GetById(ticketId);
        }

        public TicketMasterUser GetUserById(int userId)
        {
            return _ticketMasterUserRepository.GetById(userId);
        }

        public TicketMasterUser GetUserByName(string username)
        {
            return _ticketMasterUserRepository.GetUserByName(username);
        }

        public EntertainmentAddress GetEntertainmentAddressByUsername(string username)
        {
            return _entertainmentAddressRepository.GetEntertainmentAddressByUsername(username);
        }

        public bool UpdateEntertainmentAddressByUsername(string username, EntertainmentAddress entertainmentAddress)
        {
            try
            {
                var user = GetUserByName(username);
                var dbEntertainmentAddress = GetEntertainmentAddressByUsername(username);

                dbEntertainmentAddress.AddressLine1 = entertainmentAddress.AddressLine1;
                dbEntertainmentAddress.AddressLine2 = entertainmentAddress.AddressLine2;
                dbEntertainmentAddress.Town = entertainmentAddress.Town;
                dbEntertainmentAddress.PostCode = entertainmentAddress.PostCode;
                dbEntertainmentAddress.Country = entertainmentAddress.Country;
                dbEntertainmentAddress.UserId = user.UserId;
                _entertainmentAddressRepository.Update(dbEntertainmentAddress);
                _entertainmentAddressUnitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AddNewClientAddress(string username, EntertainmentAddress entertainmentAddress)
        {
            try
            {
                var user = GetUserByName(username);
                entertainmentAddress.UserId = user.UserId;

                _entertainmentAddressRepository.Add(entertainmentAddress);
                _entertainmentAddressUnitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Booking GetTicketBookingById(int id)
        {
            return _bookingRepository.GetById(id);
        }

        public void UpdateBooking(Booking booking)
        {
            _bookingRepository.Update(booking);
            _bookingUnitOfWork.SaveChanges();
        }

        public GroupedBooking[] GetBookingsByEvent(DateTime fro, DateTime to)
        {
            return _bookingRepository.GetBookingsByEvent(fro, to);
        }
    }

    public interface IRepositoryTicketServiceSegregator
    {
        Event[] GetAllEvents();

        Event[] GetAllCurrentEvents();

        Ticket[] GetAllTickets();

        Ticket[] GetTicketForUser(string username);

        int BookTickets(Ticket ticket, int numberOfTickets, int userId);

        Event GetEventById(int eventId);

        Ticket GetTicketById(int ticketId);

        TicketMasterUser GetUserById(int userId);

        TicketMasterUser GetUserByName(string username);

        EntertainmentAddress GetEntertainmentAddressByUsername(string username);

        bool UpdateEntertainmentAddressByUsername(string username, EntertainmentAddress EntertainmentAddress);

        bool AddNewClientAddress(string username, EntertainmentAddress EntertainmentAddress);

        Booking GetTicketBookingById(int id);

        void UpdateBooking(Booking booking);

        GroupedBooking[] GetBookingsByEvent(DateTime fro, DateTime to);
    }
}
