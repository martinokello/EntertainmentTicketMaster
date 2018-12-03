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
        private BookingRepository _bookingRepository;
        private TicketMasterUserRepository _ticketMasterUserRepository;
        private EventRepository _eventRepository;
        private EntertainmentAddressRepository _EntertainmentAddressRepository;

        public RepositoryTicketServices()
        {
            
        }
        public RepositoryTicketServices(ITicketRepositorySegregator ticketRepository, IEventRepositorySegregator eventRepository, ITicketMasterUserRepositorySegregator ticketMasterUserRepository, IBookingRepositorySegregator bookingRepository, IEntertainmentAddressRepositorySegregator EntertainmentAddressRepository)
        {
            _ticketRepository = ticketRepository as TicketRepository;
            _bookingRepository = bookingRepository as BookingRepository;
            _ticketMasterUserRepository = ticketMasterUserRepository as TicketMasterUserRepository;
            _eventRepository = eventRepository as EventRepository;
            _EntertainmentAddressRepository = EntertainmentAddressRepository as EntertainmentAddressRepository;
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
                booking.Ticket.EventId = booking.EventId;
                booking.Ticket.TicketId = (int)booking.TicketId;
                booking.Ticket.Event = GetEventById((int)booking.Ticket.EventId);
                tickets.Add(booking.Ticket);
            }

            return tickets.ToArray();
        }
        public int BookTickets(Ticket ticket, int numberOfTickets, int userId)
        {
            return _bookingRepository.BookTickets(ticket, numberOfTickets, userId);
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
            return _EntertainmentAddressRepository.GetEntertainmentAddressByUsername(username);
        }

        public bool UpdateEntertainmentAddressByUsername(string username, EntertainmentAddress EntertainmentAddress)
        {
            try
            {
                var user = GetUserByName(username);
                var dbEntertainmentAddress = GetEntertainmentAddressByUsername(username);

                dbEntertainmentAddress.AddressLine1 = EntertainmentAddress.AddressLine1;
                dbEntertainmentAddress.AddressLine2 = EntertainmentAddress.AddressLine2;
                dbEntertainmentAddress.Town = EntertainmentAddress.Town;
                dbEntertainmentAddress.PostCode = EntertainmentAddress.PostCode;
                dbEntertainmentAddress.Country = EntertainmentAddress.Country;
                dbEntertainmentAddress.UserId = user.UserId;
                _EntertainmentAddressRepository.Update(dbEntertainmentAddress);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AddNewClientAddress(string username, EntertainmentAddress EntertainmentAddress)
        {
            try
            {
                var user = GetUserByName(username);
                EntertainmentAddress.UserId = user.UserId;

                _EntertainmentAddressRepository.Add(EntertainmentAddress);
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
