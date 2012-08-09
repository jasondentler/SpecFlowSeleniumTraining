using System.Web.Mvc;
using MvcContrib;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public RedirectToRouteResult Index()
        {
            return this.RedirectToAction<WidgetsController>(c => c.List());
        }

    }
}
