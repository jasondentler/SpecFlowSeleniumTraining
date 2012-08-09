using System;
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
        public ViewResult List()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ViewResult Index(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet, ModelStateToTempData]
        public ViewResult Create()
        {
            return View(new WidgetDetails());
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
