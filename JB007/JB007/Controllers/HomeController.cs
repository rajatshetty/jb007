using System.Web.Mvc;

namespace JB007.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //if (Session["User"] == null)
              if(User.Identity.Name == null)
                return RedirectToAction("Login", "Account");
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

    }
}