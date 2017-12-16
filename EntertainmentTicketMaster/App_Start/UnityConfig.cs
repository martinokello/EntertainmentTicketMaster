using System.Web.Mvc;
using EmailServices;
using EmailServices.Interfaces;
using RepositoryServices.Services;
using TicketMasterDataAccess.ConcreteRepositories;
using System.Web.Http;
using Unity;
using Unity.AspNet.Mvc;

namespace EntertainmentTicketMaster
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IRepositoryTicketServiceSegregator, RepositoryTicketServices>();
            container.RegisterType<IAccontManagerServiceSegregator, AccountManagementServices>();
            container.RegisterType<IRepositoryAdminServiceSegregator, RepositoryAdminServices>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IUserRepositorySegregator, UserRepository>();
            container.RegisterType<ITicketRepositorySegregator, TicketRepository>();
            container.RegisterType<ITicketMasterUserRepositorySegregator, TicketMasterUserRepository>();
            container.RegisterType<IEventRepositorySegregator, EventRepository>();
            container.RegisterType<IBookingRepositorySegregator, BookingRepository>();
            container.RegisterType<IEntertainmentAddressRepositorySegregator, EntertainmentAddressRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}          
