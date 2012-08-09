using System.Web.Mvc;
using MvcContrib.Filters;
using UI.DataAccess;
using UI.Models;
using MvcContrib;

namespace UI.Controllers
{
    public class WidgetsController : Controller
    {
        private readonly WidgetDao _dao;

        public WidgetsController(WidgetDao dao)
        {
            _dao = dao;
        }

        [HttpGet]
        public ActionResult List()
        {
            return Content(string.Empty);
        }

        [HttpGet]
        public ViewResult Index(int id)
        {
            var widget = _dao.Get(id);
            return View(widget);
        }

        [HttpGet, ModelStateToTempData]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost, ModelStateToTempData]
        public RedirectToRouteResult Create(WidgetDetails model)
        {
            if (!ModelState.IsValid)
                return this.RedirectToAction(c => c.Create());

            var id = _dao.Create(model);

            return this.RedirectToAction(c => c.Index(id));
        }

    }
}
