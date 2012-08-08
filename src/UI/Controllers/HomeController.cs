using System.Web.Mvc;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public RedirectToRouteResult Index()
        {
            return View();
        }

    }
}
