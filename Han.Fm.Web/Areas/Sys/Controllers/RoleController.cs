using Han.Fm.Service.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Han.Fm.Web.Areas.Sys.Controllers
{
    public class RoleController : Controller
    {

        private readonly RoleService roleService = new RoleService();

        public ActionResult GetRoleManageView()
        {
            return View("~/Views/Sys/RoleManage.cshtml");
        }

        public ActionResult GetRoles(string sEcho, decimal iDisplayLength)
        {
            var result = roleService.GetRoles();

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = result.Result.Count,
                iTotalDisplayRecords = iDisplayLength,
                aaData = result.Result

            }, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult CreateRole()
        {
            return null;
        }
    }
}