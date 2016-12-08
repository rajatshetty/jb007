using System;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Security;
using JB007.Models;
using Newtonsoft.Json;

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
        public ActionResult Login(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(TransLoginModel login, string ReturnUrl)
        {
            try
            {
                string password = string.Empty, username = string.Empty;
                username = login.objLoginModel.EmailAddress;
                password = login.objLoginModel.Password;

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

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
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
    }
}