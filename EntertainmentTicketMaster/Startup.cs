using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EntertainmentTicketMaster.Startup))]
namespace EntertainmentTicketMaster
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
