using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;


namespace UPAEventsPayPal
{
    public class URLBuilder
    {
        private System.Web.HttpUtility URLUtility = null;
        private System.Web.SessionState.HttpSessionState Session;
        private string businessEmail;
        private string successUrl;
        private string cancelUrl;
        private string notifyUrl;
        private string clientEmail;
        private int invoiceNo;

        public URLBuilder(System.Web.SessionState.HttpSessionState Session, string businessEmail, string successUrl, string cancelUrl, string notifyUrl, string clientEmail, int invoiceNo)
        {
            URLUtility = new HttpUtility();
            this.businessEmail = businessEmail;
            this.clientEmail = clientEmail;
            this.Session=Session;
            this.cancelUrl = cancelUrl;
            this.successUrl = successUrl;
            this.notifyUrl = notifyUrl;
            this.invoiceNo = invoiceNo;
        }

        public string getFullCommandParameters()
        {
            StringBuilder sbUrl = new StringBuilder();
            Invoice invoice = (Invoice)Session["Invoice"];
            sbUrl.Append("cmd=_cart&upload=1");
            sbUrl.AppendFormat("&business={0}",HttpUtility.UrlEncode(businessEmail));

            int index = 1;
            foreach (Product prod in invoice)
            {
                sbUrl.AppendFormat("&item_name_{0}={1}", index.ToString(), HttpUtility.UrlEncode(prod.ProductName));
                sbUrl.AppendFormat("&amount_{0}={1}", index.ToString(), HttpUtility.UrlEncode(prod.VATAmmount.ToString()));
                sbUrl.AppendFormat("&quantity_{0}={1}", index.ToString(), HttpUtility.UrlEncode(prod.Quantity.ToString()));
                index++;
            }
            sbUrl.AppendFormat("&amount={0}", HttpUtility.UrlEncode(((decimal)Session["grossAmmount"]).ToString()));
            sbUrl.AppendFormat("&invoice={0}", HttpUtility.UrlEncode(invoiceNo.ToString()));
            sbUrl.AppendFormat("&return={0}&username={1}", HttpUtility.UrlEncode(successUrl),HttpUtility.UrlEncode((string)Session["Username"]));
            sbUrl.AppendFormat("&cancel_return={0}&username={1}", HttpUtility.UrlEncode(cancelUrl), HttpUtility.UrlEncode((string)Session["Username"]));
            sbUrl.AppendFormat("&notify_url={0}", HttpUtility.UrlEncode(notifyUrl));
            sbUrl.AppendFormat("&buyer_email={0}", HttpUtility.UrlEncode(clientEmail));

            return sbUrl.ToString();
        }
    }
}
