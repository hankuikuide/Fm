using Han.Fm.Service.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Han.Fm.Model.Dto.Sys;
using System.Reflection;

namespace Han.Fm.Web.Areas.Sys.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService userService = new UserService();

        public ActionResult GetUserListView()
        {
            return View("~/Views/Sys/UserList.cshtml");
        }

        public ActionResult CreateUser()
        {
            return View("~/Views/Sys/CreateUser.cshtml");
        }

        public ActionResult GetUsers(string sEcho, decimal iDisplayLength)
        {
            var users = userService.GetUsers();

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = users.Result.Count,
                iTotalDisplayRecords = iDisplayLength,
                aaData = users.Result

            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult RemoveUser(string userId)
        {
            var result = userService.RemoveUser(userId);
            return Json(result);
        }


        private List<string> GetPropertyList(UserResult user)
        {
            var propertyList = new List<string>();
            var properties = user.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                object o = property.GetValue(user, null);
                propertyList.Add(o == null ? "" : o.ToString());
            }
            return propertyList;
        }


    }
}