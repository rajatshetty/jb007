using System;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using JB007.Models;
using JB007.Models.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

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
                    var currentUser = _response.Content.ReadAsAsync<JB007.Models.LoginModel>().Result;
                    if (currentUser != null)
                    {
                        var identity = new ClaimsIdentity(new[] {
                             new Claim(ClaimTypes.Name,currentUser.EmailAddress),
                            new Claim(ClaimTypes.Email,currentUser.EmailAddress),
                            new Claim("TenentId",  currentUser.TenantId.ToString())
                        },
                       "ApplicationCookie");                      

                        var ctx = Request.GetOwinContext();
                        var authManager = ctx.Authentication;

                        authManager.SignIn(identity);

                        //FormsAuthentication.SetAuthCookie(login.objLoginModel.EmailAddress, true);
                        //Session["User"] = CurrentUser.EmailAddress;
                        //Session["CurrentTenentId"] = CurrentUser.TenantId;
                        //Session["UserId"] = CurrentUser.Id;

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
            //Session.Abandon();
            //// Delete the authentication ticket and sign out.
            //FormsAuthentication.SignOut();
            //// Clear authentication cookie.
            //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            //cookie.Expires = DateTime.Now.AddYears(-1);
            //Response.Cookies.Add(cookie);
            //return RedirectToAction("Login", "Account");

            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Account");
        }
        #region Registration
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
        #endregion
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }else
            {
                var identity = new ClaimsIdentity(new[] {
                             new Claim(ClaimTypes.Name,loginInfo.ExternalIdentity.Name),
                            new Claim(ClaimTypes.Email,loginInfo.Email)
                        },
                      "ApplicationCookie");

            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignIn(identity);

            return RedirectToAction("Index", "Home");
            }
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}