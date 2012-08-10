using System;
using System.Web.Mvc;
using Ninject;
using Raven.Client;

namespace UI
{
    public class RavenGlobalFilter : IActionFilter
    {
        private readonly IDocumentSession _session;

        public RavenGlobalFilter(IDocumentSession session)
        {
            _session = session;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction && filterContext.Exception == null)
            {
                _session.SaveChanges();
            }
            _session.Dispose();
        }
    }
}