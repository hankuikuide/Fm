using Han.Fm.Model.BaseDto;
using Han.Fm.Model.Dto.Account;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Han.Fm.Web.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Login()
        {
            return View();
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginOn(LoginIn model)
        {
            Response<bool> result = new Response<bool>();

            SetCookie(JsonConvert.SerializeObject(model));

            return Json(result);

        }

        public void SetCookie(string source)
        {

            var ticket = new FormsAuthenticationTicket(1, "loginuser", DateTime.Now, DateTime.Now.AddDays(10), false, source);

            string authTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie("hanApp", authTicket);

            Response.Cookies.Remove(cookie.Name);
            Response.Cookies.Add(cookie);

            HttpCookie signCookie = new HttpCookie("hanApp_sign", "1  ");

            Response.Cookies.Remove(signCookie.Name);
            Response.Cookies.Add(signCookie);
        }
    }
}