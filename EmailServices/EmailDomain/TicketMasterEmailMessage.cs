using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailServices.EmailDomain
{
    public class TicketMasterEmailMessage
    {
        public string EmailMessage { get; set; }
        public IList<string> EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string AttachmentFilePath { get; set; }
        public string Subject { get; set; }
    }
}
