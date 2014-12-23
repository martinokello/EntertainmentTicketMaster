using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntertainmentTicketMaster.Models
{
    public class TicketViewModel
    {
        
        public Guid TicketGUID { get; set; }
        [Required]
        public decimal Price { get; set; }

        [Required]
        [DisplayName("Number Of Tickets")]
        public int NumberOfTickets { get; set; }

        [DisplayName("Event ID")]
        public int EventId { get; set; }

        [DisplayName("Event Names")]
        public string EventName { get; set; }

        [Required]
        [DisplayName("Total Price")]
        public decimal TotalPrice { get; set; }
    }
}