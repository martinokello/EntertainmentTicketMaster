using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntertainmentTicketMaster.Models
{
    public class BulkEmailContentViewModel
    {
        [DisplayName("Email Message")]
        [Required]
        public string EmailMessage { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }
}