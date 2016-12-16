using System;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Security;
using JB007.Models;
using JB007.Models.Models;

namespace JB007.Controllers
{
    public class AccountController : Controller
    {
        readonly HttpClient _client;
        HttpResponseMessage _response;

        public AccountController()
        {
            string strApiurl = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
            Uri serviceUri = new Uri(strApiurl);
            _client = new HttpClient();
            _client.BaseAddress = serviceUri;
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _response = new HttpResponseMessage();
        }

        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnData)
        {
            //if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
            //    returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            //if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            //{
            //    ViewBag.ReturnURL = returnUrl;
            //}
            if (string.IsNullOrEmpty(returnData))
                ViewBag.ReturnData = returnData;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(TransLoginModel login, string returnUrl)
        {
            try
            {
                string password = string.Empty, username = string.Empty;
                username = login.objLoginModel.EmailAddress;
                password = Common.Encrypt(login.objLoginModel.Password);

                _response = _client.GetAsync("api/Account/ValidateUser?username=" + username + "&password=" + password).Result;
                if (_response.IsSuccessStatusCode)
                {
                    var CurrentUser = _response.Content.ReadAsAsync<JB007.Models.LoginModel>().Result;
                    if (CurrentUser != null)
                    {
                        FormsAuthentication.SetAuthCookie(login.objLoginModel.EmailAddress, true);
                        Session["User"] = CurrentUser.EmailAddress;
                        Session["CurrentTenentId"] = CurrentUser.TenantId;
                        Session["UserId"] = CurrentUser.Id;

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ModelState.Clear();
            return View(login);

        }

        public ActionResult LogoutMethod()
        {
            Session.Abandon();

            // Delete the authentication ticket and sign out.
            FormsAuthentication.SignOut();

            // Clear authentication cookie.
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Registration()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Registration(RegistrationModel register)
        {
            if (ModelState.IsValid)
            {
                register.CreatedDate = DateTime.Now;
                register.TenantId = Guid.NewGuid();
                register.Password = Common.Encrypt(register.Password);
                _response = _client.PostAsJsonAsync("api/Account/Registration", register).Result;
                if (_response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Account", new { ReturnData = "Registration Got Sucesss" });
                }
            }
            return View(register);
        }
    }
}