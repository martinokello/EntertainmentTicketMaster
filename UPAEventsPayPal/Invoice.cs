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
    public class Invoice:IEnumerable<Product>
    {
        private int invoiceNo;
        private List<Product> products;
        private string buyerEmail;
        private decimal ammount;
        private decimal ammountVAT;
        private const int beginGenerate = 1003;
        static private Random randomGenerator = new Random(DateTime.Now.Millisecond);

        public Invoice(List<Product> products, decimal ammount, string buyerEmail)
        {
            this.products = products;
            this.ammount = ammount;
            this.buyerEmail = buyerEmail;
        }

        public long GenerateUniqueInvoiceNo()
        {
            invoiceNo = randomGenerator.Next(beginGenerate);
            return (long)invoiceNo;
        }

        public int InvoiceNo
        {
            get { return invoiceNo; }
            set { invoiceNo = value; }
        }

        public string BuyerEmail
        {
            get { return buyerEmail; }
            set { buyerEmail = value; }
        }


        public List<Product> Products
        {
            get { return products; }
            set { products = value; }
        }

        public decimal Ammount
        {
            get { return ammount; }
            set { ammount = value; }
        }

        public decimal AmmountVAT
        {
            get { return ammountVAT; }
            set { ammountVAT = value; }
        }

        public Product this[int index]
        {
            get{
                if (index > products.Count || index < 0) throw new IndexOutOfRangeException("The Index is out of Range");          
                return products[index];
            }
            set
            {
                if (index > products.Count || index < 0) throw new IndexOutOfRangeException("The Index is out of Range");
                products[index] = value;
            }
        }
        public IEnumerator<Product> GetEnumerator()
        {
            return products.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public decimal CalculateGrossAmmountWithVAT(double VAT)
        {
            double ammount = 0.0;
            foreach (Product prod in products)
            {
                ammount += (double)(prod.Quantity * prod.Ammount);
            }
            Ammount = (decimal)Math.Round(ammount, 2);
            AmmountVAT = (decimal)Math.Round(ammount * VAT, 2);
            return AmmountVAT;
        }
        public void CalculateItemAmmountVAT(Product prod, double VAT)
        {
            decimal unitPrice = prod.Ammount;
            decimal withVATPrice = unitPrice + (unitPrice * (decimal)VAT);
            prod.VATAmmount=Math.Round(withVATPrice,2);
        }
    }
}
