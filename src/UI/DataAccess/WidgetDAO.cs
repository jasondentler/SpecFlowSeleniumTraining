using System;
using UI.Models;

namespace UI.DataAccess
{
    public class WidgetDao
    {

        public int Create(WidgetDetails newWidget)
        {
            return newWidget.Name.GetHashCode();
        }

    }
}