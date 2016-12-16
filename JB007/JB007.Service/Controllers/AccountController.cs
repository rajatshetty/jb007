using System;
using System.Web.Http;
using System.Linq;
using System.Net;
using System.Net.Http;
using AutoMapper;
using JB007.Entities;
using JB007.Models.Models;

namespace JB007.Service.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private JB007Entities _dbcontext = new JB007Entities();

        [HttpGet]
        [Route("ValidateUser")]
        public IHttpActionResult ValidateUser(string username, string password)
        {
            try
            {
                var CurrentUser = (from cu in _dbcontext.Logins
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
                return new TextResult(ex.Message, Request);
            }
        }

        #region Post
        [HttpPost]
        [Route("Registration")]
        public IHttpActionResult Post([FromBody]RegistrationModel objRegister)
        {
            try
            {
                var registerDTO = Mapper.Map<RegUser>(objRegister);

               if (!_dbcontext.RegUsers.Any(u =>  u.UserEmail == objRegister.UserEmail))
                {
                    _dbcontext.RegUsers.Add(registerDTO);
                    _dbcontext.Logins.Add(new Login()
                    {
                        EmailAddress = registerDTO.UserEmail,
                        Password = registerDTO.Password,
                        TenantId = registerDTO.TenantId
                    });
                    _dbcontext.SaveChanges();
                }

                return Ok(objRegister);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                return new TextResult(ex.Message, Request);
            }
        }

        #endregion Post
    }
}
