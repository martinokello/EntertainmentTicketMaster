using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMasterDataAccess.Dto
{
    public class GroupedBooking
    {
        public int NumberOfTickets { get; set; }
        public int BookingId { get; set; }
        public string EventName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class BookingStats
    {
        public int NumberOfTickets { get; set; }
        public decimal TotalAmount { get; set; }
        public int BookingDate { get; set; }
    }
}
