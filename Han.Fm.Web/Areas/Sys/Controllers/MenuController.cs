using Han.Fm.Model.Dto.Sys;
using Han.Fm.Service.Sys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Han.Fm.Web.Areas.Sys.Controllers
{
    public class MenuController : Controller
    {
        private readonly MenuService menuService = new MenuService();
 
        public ActionResult GetMenus()
        {
            var result = menuService.GetMenus();

            return Json(result);
            
        }

        public ActionResult GetMenusContent(string sEcho, decimal iDisplayLength)
        {
            var result = menuService.GetMenus();

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = result.Result.Count,
                iTotalDisplayRecords = iDisplayLength,
                aaData = result.Result

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetMenuManageView()
        {
            return View("~/Views/Sys/MenuManage.cshtml");

        }

        public ActionResult GetMenuNodeView(string menunode)
        {
             var result = JsonConvert.DeserializeObject<MenuResult>(menunode);

            return View("~/Views/Sys/MenuNode.cshtml", result);
        }

        public ActionResult SaveMenu(string menunode)
        {
            var menu = JsonConvert.DeserializeObject<MenuResult>(menunode);

            var result = menuService.SaveMenu(menu);

            return Json(result);
        }

        public ActionResult RemoveMenu(decimal menuId)
        {
            var result = menuService.RemoveMenu(menuId);
            return Json(result);
        }
    }
}