using Han.Fm.Service.Sys;
using Han.Fm.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Han.Fm.Web.Controllers
{
    [Validate]
    public class HomeController : Controller
    {
        private readonly MenuService menuService = new MenuService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}