using Han.Fm.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace Han.Fm.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ActionExceptionAttribute());
        }
    }
}
