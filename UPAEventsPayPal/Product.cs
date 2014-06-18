using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace UPAEventsPayPal
{
    public class Product
    {
        private decimal ammount;
        private decimal ammountVAT;
        private string prodName;
        private string prodDescription;
        private int quantity;


        public Product()
        {
            
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public decimal Ammount
        {
            get { return ammount; }
            set { ammount = value; }
        }
        public decimal VATAmmount
        {
            get { return ammountVAT; }
            set { ammountVAT = value; }
        }

        public string ProductName
        {
            get { return prodName; }
            set { prodName = value; }
        }

        public string ProductDescription
        {
            get { return prodDescription; }
            set { prodDescription = value; }
        }
    }
}
