using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using EmailServices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Http;
using EntertainmentTicketMaster.Models;
using Microsoft.AspNet.Identity;
using RepositoryServices.Services;
using TicketMasterDataAccess.DataAccess;

namespace EntertainmentTicketMaster.Controllers
{
    public class EmailListController : ApiController
    {
        private RepositoryAdminServices _adminServices;

        private EmailService _emailServices;

        public EmailListController()
        {
            _adminServices = new RepositoryAdminServices();
        }

        // POST api/<controller>
        [HttpPost]
        public IHttpActionResult AddUserToMailList(RegisterViewModel model)
        {
            var emailStart = model.Email.Split(new char[] {'@','.'},StringSplitOptions.RemoveEmptyEntries)[0];
            var makeUniqueUsername = emailStart + model.UserName;
            var pattern = @"(_| |-|\.)";
            var regEx = new Regex(pattern);
            makeUniqueUsername = regEx.Replace(makeUniqueUsername,"");
            var user = new ApplicationUser() { UserName = makeUniqueUsername };
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            IdentityResult result;
            try
            {
                result = userManager.Create(user, model.Password);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.Conflict, "User was not added successfully to user list due to Error");
            }
            

            if (result != null && result.Succeeded)
            {
                var userOfDomain = new TicketMasterUser
                {
                    ASPNetUserId = user.Id,
                    Email = model.Email,
                    UserName = makeUniqueUsername
                };

                _adminServices.AddUser(userOfDomain);
                return Ok("User successfully added to the email list");
            }
            else return Content(HttpStatusCode.Conflict, "User was not added successfully to user list");
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}