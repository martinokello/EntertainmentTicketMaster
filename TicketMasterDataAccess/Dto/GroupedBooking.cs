using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMasterDataAccess.Dto
{
    public class GroupedBooking
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int NumberOfBookings { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
