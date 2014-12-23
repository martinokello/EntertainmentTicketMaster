using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntertainmentTicketMaster.Models
{
    public class EntertainmentAddressViewModel
    {
        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        [Required]
        public string Town { get; set; }
        [Required]
        public string PostCode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}