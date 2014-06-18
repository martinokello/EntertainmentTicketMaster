using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntertainmentTicketMaster.Models
{
    public class EventViewModel
    {
        public string Location { get; set; }
        public int EventId { get; set; }
        [Required]
        [DisplayName("Event Name")]
        public string EventName { get; set; }

        [Required]
        [DisplayName("Event Time hh:mm")]
        public string EventTime { get; set; }

        [Required]
        [DisplayName("Event Description")]
        public string EventDescription { get; set; }
        [DisplayName("Event Date")]
        [Required]
        public string EventDate { get; set; }
        [DisplayName("Number Of Tickets")]
        public int NumberOfTickets { get; set; }

        [Required]
        [DisplayName("Price Per Ticket")]
        public decimal Price { get; set; }

        public HttpPostedFileBase Attachment { get; set; }
    }
}