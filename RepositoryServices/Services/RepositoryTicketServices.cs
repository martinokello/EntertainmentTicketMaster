using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.DataAccess;
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
        private AddressRepository _addressRepository;

        public RepositoryTicketServices()
        {
            
        }
        public RepositoryTicketServices(ITicketRepositorySegregator ticketRepository, IEventRepositorySegregator eventRepository, ITicketMasterUserRepositorySegregator ticketMasterUserRepository, IBookingRepositorySegregator bookingRepository, IAddressRepositorySegregator addressRepository)
        {
            _ticketRepository = ticketRepository as TicketRepository;
            _bookingRepository = bookingRepository as BookingRepository;
            _ticketMasterUserRepository = ticketMasterUserRepository as TicketMasterUserRepository;
            _eventRepository = eventRepository as EventRepository;
            _addressRepository = addressRepository as AddressRepository;
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

        public Address GetAddressByUsername(string username)
        {
            return _addressRepository.GetAddressByUsername(username);
        }

        public bool UpdateAddressByUsername(string username, Address address)
        {
            try
            {
                var user = GetUserByName(username);
                var dbAddress = GetAddressByUsername(username);

                dbAddress.AddressLine1 = address.AddressLine1;
                dbAddress.AddressLine2 = address.AddressLine2;
                dbAddress.Town = address.Town;
                dbAddress.PostCode = address.PostCode;
                dbAddress.Country = address.Country;
                dbAddress.UserId = user.UserId;
                _addressRepository.Update(dbAddress);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AddNewClientAddress(string username, Address address)
        {
            try
            {
                var user = GetUserByName(username);
                address.UserId = user.UserId;

                _addressRepository.Add(address);
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
    }

    public interface IRepositoryTicketServiceSegregator
    {
        Event[] GetAllEvents();

        Ticket[] GetAllTickets();

        Ticket[] GetTicketForUser(string username);
        int BookTickets(Ticket ticket, int numberOfTickets, int userId);

        Event GetEventById(int eventId);

        Ticket GetTicketById(int ticketId);

        TicketMasterUser GetUserById(int userId);
        TicketMasterUser GetUserByName(string username);
        Address GetAddressByUsername(string username);
        bool UpdateAddressByUsername(string username, Address address);

        bool AddNewClientAddress(string username, Address address);

        Booking GetTicketBookingById(int id);
        void UpdateBooking(Booking booking);
    }
}
