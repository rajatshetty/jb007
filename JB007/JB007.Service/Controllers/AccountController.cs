using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Linq;
using System.Web.Http.ModelBinding;
using JB007.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using JB007.Service.Models;
using JB007.Service.Providers;
using JB007.Service.Results;

namespace JB007.Service.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private JB007Entities dbcontext = new JB007Entities();
        public AccountController()
        {
        }

        [HttpGet]
        [Route("ValidateUser")]
        public IHttpActionResult ValidateUser(string username, string password)
        {
            try
            {
                var CurrentUser = (from cu in dbcontext.Logins
                    where cu.EmailAddress == username && cu.Password == password && cu.Status == true
                    select
                    new
                    {
                        Id = cu.Id,
                        TenantId = cu.TenantId,
                        EmailAddress = cu.EmailAddress,
                        CreatedDate = cu.CreatedDate,
                        ModifiedDate = cu.ModifiedDate
                    }).FirstOrDefault();
                return Json(CurrentUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
