using System.Web.Mvc;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Raven.Client;
using Raven.Client.Document;

namespace UI.App_Start
{
    public class RavenModule : NinjectModule 
    {
        public override void Load()
        {
            var store = new DocumentStore()
                            {
                                ConnectionStringName = "RavenDb"
                            };

            store.Initialize();

            Kernel.Bind<IDocumentSession>()
                .ToMethod(ctx => store.OpenSession())
                .InRequestScope();

            Kernel.BindFilter<RavenGlobalFilter>(FilterScope.Controller, 0);
        }
    }
}