using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMasterDataAccess.Dto
{
    public class BookingTicketInfo
    {
        public string Username { get; set; }
        public string EventName { get; set; }
        public int NumberOfTickets { get; set; }
        public bool IsVerifiedPayment { get; set; }
        public int BookingId { get; set; }
    }
}
