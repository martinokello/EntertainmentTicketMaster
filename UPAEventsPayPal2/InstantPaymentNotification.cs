using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using EmailServices.EmailDomain;
using RepositoryServices.Services;
using TicketMasterDataAccess.ConcreteRepositories;


namespace UPAEventsPayPal
{
    public class InstantPaymentNotification
    {
        private HttpWebRequest httpWebRequest;
        private HttpRequest request;
        private string accountEmail;
        private string clientEmail;
        private DateTime orderDate;
        private NameValueCollection form;
        private BookingRepository _bookingRepository;

        public InstantPaymentNotification(HttpRequest request, string accountEmail, NameValueCollection form, BookingRepository repositoryTicket)
        {
            this.request = request;
            this.accountEmail = accountEmail;
            this.form = form;
            _bookingRepository = repositoryTicket;
        }

        public string ClientEmail
        {
            get { return clientEmail; }
            set { clientEmail = value; }
        }
        public DateTime OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }
        public bool ProcessIPNResults(HttpContext context, StreamWriter IPNWriter)
        {
            bool result = false;
            // *** Reload our invoice and try to match it

            // *** a real invoice

            int InvoiceNo = Int32.Parse(form["invoice"]);
            decimal amount = Convert.ToDecimal(form["mc_gross"]);
            clientEmail = null;
            int orderId = -1;
            var orderAmmount = new decimal();
            var Username = string.Empty;
            var Password = string.Empty;


            //Get Client Order from DB:
            var booking = _bookingRepository.GetById(InvoiceNo); 



            clientEmail = form["payer_email"];

            if (!form["business"].Equals(System.Configuration.ConfigurationManager.AppSettings["BusinessEmail"])) result = false;

            if (booking.BookingId != InvoiceNo) result = false;

            orderAmmount = (decimal)(booking.NumberOfTickets * booking.Ticket.Price);
            if (orderAmmount != amount) result = false;

            // *** Send the response data back to PayPal for confirmation

            StringBuilder sbUrl=new StringBuilder();
            sbUrl.Append("cmd=_notify-validate");

            StringBuilder prodBuffer = new StringBuilder();
            string username = null;
            string buyerEmail = null;

            foreach (string postKey in form)
            {
                sbUrl.Append("&"+postKey + "=" + form[postKey]);
                if (postKey.StartsWith("item_")) prodBuffer.Append(form[postKey] + "_");
                if (postKey.StartsWith("buyer_email")) buyerEmail = form[postKey];
                if (postKey.StartsWith("username")) username = form[postKey];
            }

            string requestUriString = System.Configuration.ConfigurationManager.AppSettings["PaypalBaseUrl"];

            this.httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(requestUriString);
            // Set values for the request back
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Timeout = 10000;
            // *** Set properties



            //retrieve post string:

            byte[] lbPostBuffer = Encoding.GetEncoding(1252).GetBytes(sbUrl.ToString());
            httpWebRequest.ContentLength = lbPostBuffer.Length;

            Stream loPostData = httpWebRequest.GetRequestStream();
            loPostData.Write(lbPostBuffer, 0, lbPostBuffer.Length);

            loPostData.Close();

            HttpWebResponse loWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            Encoding enc = Encoding.GetEncoding(1252);

            StreamReader loResponseStream = new StreamReader(loWebResponse.GetResponseStream(), enc);
            string verify = loResponseStream.ReadToEnd();

            loWebResponse.Close();
            loResponseStream.Close();

            IPNWriter.WriteLine(DateTime.Now.ToString("dd/MM/yyyy") + ":  " + verify);
            IPNWriter.Flush();
            IPNWriter.Close();

            if (verify.Contains("VERIFIED"))
            {
                booking.IsVerifiedPayment = true;
                _bookingRepository.Update(booking);
                 //Send Confirmation Mail

                    var emailPassword = System.Configuration.ConfigurationManager.AppSettings["emailPassword"];
                    var smtpUsername = System.Configuration.ConfigurationManager.AppSettings["BusinessEmail"];
                    var smtpClient = System.Configuration.ConfigurationManager.AppSettings["SmtpHostServer"];
                    //smtpClient.Credentials = new NetworkCredential {Password = emailPassword, UserName = smtpUsername };

                var message = new TicketMasterEmailMessage
                {
                    AttachmentFilePath = null,
                    EmailFrom = smtpUsername,
                    EmailMessage =
                        string.Format(
                            "This is confirmation that client {0} tickets will be dispatched as Paypal has verified transaction.",
                            username),
                    Subject = "Confirmation from Paypal payment gateway.",
                    EmailTo = new List<string>()
                }; 
                     
                    message.EmailTo.Add("martin.okello@martinlayooinc.co.uk");
                    message.EmailTo.Add(smtpUsername);
                    message.EmailTo.Add(clientEmail);

                    var smtpServer = new EmailServices.EmailService(smtpClient);
                smtpServer.SendEmail(message);
                    return true;
            }
            return false;
        }


    }
}
