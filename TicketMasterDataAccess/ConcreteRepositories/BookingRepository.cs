using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TicketMasterDataAccess.Abstracts;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Dto;

namespace TicketMasterDataAccess.ConcreteRepositories
{
    public class BookingRepository : AbstractTicketRepository<Booking, int>, IBookingRepositorySegregator
    {
        public BookingRepository()
        {
        }
        public override bool Add(Booking instance)
        {
            var result = base.Add(instance);
            return result;
        }
        public override Booking GetById(int key)
        {
            return DBContext.Bookings.SingleOrDefault(p => p.BookingId == key);
        }

        public override bool Delete(int key)
        {
                try
                {
                DBContext.Bookings.Remove(
                        DBContext.Bookings.SingleOrDefault(k => k.BookingId == key));
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }

        public override bool Update(Booking instance)
        {
                try
                {
                    var booking =
                        DBContext.Bookings.SingleOrDefault(k => k.BookingId == instance.BookingId);
                    booking.BookingDate = instance.BookingDate;
                    booking.EventId = instance.EventId;
                    booking.NumberOfTickets = instance.NumberOfTickets;
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }

        public int BookTickets(Ticket ticket,int numberOfTickets,int userId)
        {
                try
                {
                     DBContext.Tickets.Add(ticket);
                    var evet = DBContext.Events.SingleOrDefault(p => p.EventId == ticket.EventId);
                    ticket.Event = evet;

                    var booking = new Booking
                    {
                        BookingDate = DateTime.Now,
                        EventId = evet.EventId,
                        NumberOfTickets = numberOfTickets,
                        UserId = userId,
                        TicketId = ticket.TicketId,
                        IsVerifiedPayment = false,
                        Ticket = ticket,
                    };

                    this.Add(booking);
                    return booking.BookingId;
                }
                catch (Exception e)
                {
                    return -1;
                }
        }

        public virtual Booking[] GetTicketsForUser(string username)
        {
            var userRepository = new TicketMasterUserRepository();
            var user = userRepository.GetUserByName(username);
            return DBContext.Bookings.Where(p => p.UserId == user.UserId).ToArray();
        }

        public virtual BookingTicketInfo[] GetTicketsForUserVerified()
        {
            var userRepository = new TicketMasterUserRepository();
            var tickets = from t in DBContext.Tickets
                          from b in DBContext.Bookings
                          from u in DBContext.TicketMasterUsers
                where t.TicketId == b.TicketId && b.IsVerifiedPayment == true && u.UserId == b.UserId
                select new BookingTicketInfo
                {
                    BookingId = b.BookingId,
                    EventName = b.Ticket.Event.EventName,
                    IsVerifiedPayment = true,
                    NumberOfTickets = (int) b.NumberOfTickets,
                    Username = u.UserName
                };

            return tickets.ToArray();
        }

        public virtual GroupedBooking[] GetBookingsByEvent(DateTime fro, DateTime to)
        {
            var results = from b in DBContext.Bookings
                from t in DBContext.Tickets where b.EventId == t.EventId && b.BookingDate >= fro && b.BookingDate <= to
                orderby b.Ticket.Event.EventName
                group t by b
                into gr
                select
                    new GroupedBooking
                    {
                        BookingId = (int)gr.Key.BookingId,
                        EventName = gr.Key.Ticket.Event.EventName,
                        NumberOfTickets = (int)gr.Key.NumberOfTickets,
                        TotalAmount = (decimal) gr.Where(q => q.TicketId == gr.Key.TicketId).Select(p => p.Price * gr.Key.NumberOfTickets).FirstOrDefault(),
                        BookingDate = (DateTime)gr.Key.BookingDate
                    };
            return results.ToArray();

        }

        public virtual BookingStats[] GetStatsByMonth(DateTime fro, DateTime to)
        {
            var results = from b in DBContext.Bookings
                          from t in DBContext.Tickets
                          where b.EventId == t.EventId && b.BookingDate >= fro && b.BookingDate <= to
                          orderby b.Ticket.Event.EventName
                          group t by b.BookingDate.Value.Month
                              into gr
                              select
                                  new BookingStats
                                  {
                                      NumberOfTickets = (int)gr.Count(),
                                      TotalAmount = (decimal)gr.Select(p => p.Price * gr.Count()).FirstOrDefault(),
                                      BookingDate =gr.Key
                                  };
            return results.ToArray();
        }
    }
    public interface IBookingRepositorySegregator
    {

    }
}
