using System;
using System.Data;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.IO;
using RepositoryServices.Services;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.UnitOfWork;

namespace UPAEventsPayPal
{
    public class PayPalHttpModule : IHttpModule
    {
        public PayPalHttpModule()
        {

        }
        public String ModuleName
        {
            get { return "PayPalHttpModule"; }
        }


        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(Onapplication_BeginRequest);
        }

        public void Onapplication_BeginRequest(object sender, EventArgs e)
        {

            HttpApplication application = sender as HttpApplication;
            HttpContext context = application.Context;
            try
            {
                string myUrl = System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"];
                if (application.Request.Path.Contains("VerifyPayment"))
                {
                    var form = context.Request.QueryString;
                    InstantPaymentNotification PayPalINP = new InstantPaymentNotification(context.Request, System.Configuration.ConfigurationSettings.AppSettings["BusinessEmail"],form, new BookingRepository(new UnitOfWork()));
                    FileInfo fileInfo = new FileInfo(context.Server.MapPath("~/IPNMessage.txt"));
                    StreamWriter IPNWriter = fileInfo.CreateText();
                    bool result = PayPalINP.ProcessIPNResults(context,IPNWriter);

                    string clientEmail = PayPalINP.ClientEmail;

                    //Send Emails to me and customer about failed payment!!
                    var emailPassword = System.Configuration.ConfigurationManager.AppSettings["emailPassword"];
                    var smtpUsername = System.Configuration.ConfigurationManager.AppSettings["BusinessEmail"];
                    var smtpClient = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["smtpServer"]);
                    smtpClient.Credentials = new NetworkCredential { Password = emailPassword, UserName = smtpUsername };

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("business-enterprise@martinlayooinc.co.uk");
                    message.To.Add("martin.okello@martinlayooinc.co.uk");
                    message.To.Add(clientEmail);
                    message.Subject = "Result of your Transaction with MartinLayooInc.";

                    string status = null;

                    if (result)
                    {
                        message.Body = String.Format("Your account at Paypal at: {0} has been successfully credited with the required Payment.\nCongratulations",PayPalINP.OrderDate.ToString("dd/MM/yyyy"));
                        status = "Success";
                    }
                    else
                    {
                        message.Body = String.Format("The Transaction most recent at your Paypal Account at: {0} has failed. Sorry looks like your customer: " + clientEmail + ", is trying to fleece you!!\nSorry to be the bearer of Bad news\n\nKingPing Aka The Medallion", PayPalINP.OrderDate.ToString("dd/MM/yyyy"));

                        status = "Failed";
                    }
                    smtpClient.Send(message);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            catch (Exception ex)
            {
                string pathFile = context.Server.MapPath("~/ErrorLog.txt");
                FileInfo fileInfo = new FileInfo(pathFile);
                StreamWriter writer = fileInfo.CreateText();
                writer.WriteLine("Error Time at: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                writer.WriteLine(ex.Message);
                writer.Write(ex.StackTrace);
                writer.Close();
            }
        }

        public void Dispose()
        {

        }

    }
}
