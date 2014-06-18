using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailServices.EmailDomain;

namespace EmailServices.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(TicketMasterEmailMessage message);
    }
}
