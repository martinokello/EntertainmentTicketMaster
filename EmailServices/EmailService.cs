using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EmailServices.EmailDomain;
using EmailServices.Interfaces;

namespace EmailServices
{
    public class EmailService:IEmailService
    {
        private SmtpClient _smtpServer;

        public EmailService(string smtpHostServer)
        {
            _smtpServer = new SmtpClient(smtpHostServer);
        }

        public void SendEmail(TicketMasterEmailMessage message)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(message.EmailFrom);
            foreach(var to in message.EmailTo)
            {
                mailMessage.To.Add(new MailAddress(to));
            }
            if (!string.IsNullOrEmpty(message.AttachmentFilePath))
                mailMessage.Attachments.Add(new Attachment(message.AttachmentFilePath));

            mailMessage.Subject = message.Subject;
            mailMessage.Body = message.EmailMessage;
            _smtpServer.Credentials = new NetworkCredential("business-enterprise@martinlayooinc.co.uk", "eps1LonX!505First14Chars");

            _smtpServer.Send(mailMessage);
        }
    }
}
