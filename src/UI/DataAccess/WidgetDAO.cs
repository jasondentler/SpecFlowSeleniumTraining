using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using UI.Models;

namespace UI.DataAccess
{
    public class WidgetDao
    {
        private readonly IDocumentSession _session;

        public WidgetDao(IDocumentSession session)
        {
            _session = session;
        }

        public int Create(WidgetDetails newWidget)
        {
            _session.Store(newWidget);
            return newWidget.Id;
        }

        public WidgetDetails Get(int id)
        {
            return _session.Load<WidgetDetails>(id);
        }

        public IEnumerable<WidgetSummary> GetAll()
        {
            return _session.Query<WidgetDetails>()
                .OrderBy(detail => detail.Name)
                .Select(detail => new WidgetSummary()
                                      {
                                          Id = detail.Id,
                                          Name = detail.Name
                                      })
                .ToArray();
        }
    }
}