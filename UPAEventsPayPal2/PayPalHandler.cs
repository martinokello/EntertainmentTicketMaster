using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace UPAEventsPayPal
{
    public class PayPalHandler
    {
        private System.Web.SessionState.HttpSessionState Session;
        private bool hasBeenRedirected;
        private string baseUrl;
        private string notifyUrl;
        private string successUrl;
        private string cancelUrl;
        private Invoice invoice;
        private HttpResponse response = null;
        private string businessEmail = null;

        public PayPalHandler(System.Web.SessionState.HttpSessionState Session, string baseUrl, string businessEmail, string successUrl, string cancelUrl,string notifyUrl)
        {
            this.Session = Session;
            this.baseUrl = baseUrl;
            this.hasBeenRedirected = false;
            this.businessEmail = businessEmail;
            this.successUrl = successUrl;
            this.cancelUrl = cancelUrl;
            this.notifyUrl = notifyUrl;
        }

        public void RedirectToPayPal()
        {
            //fill In invoice Details

            List<Product> productArray = (List<Product>)Session["ProductsUPA"];
            StringBuilder prodNames = new StringBuilder();
            decimal ammount = 0;
            foreach(Product prod in productArray)
            {
                ammount += prod.Ammount;
                prodNames.Append(prod.ProductName + ";");
            }
            Session["UPAproducts"] = prodNames.ToString();
            

            invoice = new Invoice(productArray, ammount, (string)Session["buyerEmail"]);

            //Calculate Gross VAT ammount
            ammount = invoice.CalculateGrossAmmountWithVAT(0);
            
            //work out ammount to submit in Invoice to paypal:
            foreach (Product p in invoice)
            {
                invoice.CalculateItemAmmountVAT(p, 0);
            }
            //Store Info about transaction in Session
            Session["grossAmmount"] = ammount;
            
            Session["Invoice"] = invoice;
            int invoiceNo = (int)Session["InvoiceNo"];
            
            hasBeenRedirected = true;
            URLBuilder urlBuilder = new URLBuilder(Session, businessEmail, successUrl, cancelUrl, notifyUrl,(string)Session["buyerEmail"],invoiceNo);
            string requestUrl = baseUrl + urlBuilder.getFullCommandParameters();
            if (response != null)
            {
                response.Redirect(requestUrl);
            }
        }

        public HttpResponse Response
        {
            get { return response; }
            set { response = value; }
        }

        public bool HasBeenRequested
        {
            get { return hasBeenRedirected; }
            set { hasBeenRedirected = false; }
        }

        public string CancelUrl
        {
            get { return cancelUrl;  }
            set { cancelUrl=value; }
        }
        public string NotifyUrl
        {
            get { return notifyUrl; }
            set { notifyUrl = value; }
        }
        public string SuccessUrl
        {
            get { return successUrl; }
            set { successUrl = value; }
        }
    }
}
