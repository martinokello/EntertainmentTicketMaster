using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace UPAEventsPayPal
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            DateTime time = DateTime.Now;


            lblTime.Text = time.ToLocalTime().ToString("T");

            //Parse DateTime:
            try
            {
                DateTime cTime = DateTime.Parse(lblTime.Text, CultureInfo.CreateSpecificCulture("en-GB"), DateTimeStyles.AssumeLocal);
                lblTime.Text += "<div>" + cTime.ToLocalTime() + "</div>";
            }
            catch (Exception ex)
            {

            }
        }
    }
}
