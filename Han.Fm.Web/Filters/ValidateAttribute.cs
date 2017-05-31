using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Han.Fm.Model;
using Han.Fm.Model.BaseDto;

namespace Han.Fm.Web.Filters
{
    public class ValidateAttribute : ActionFilterAttribute
    {
        public ValidateAttribute()
        {

        }

        public string ValidateCode { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            var redic = new RedirectResult("Account/Login");
            if (!request.IsAuthenticated)
            {
                if (request.HttpMethod.ToUpper() != "GET")
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new Response<bool>()
                        {
                            Result = false,
                            ErrCode = "1002",
                            ErrMsg = "登录过期，请重新登录"
                        }
                    };
                    return;
                }
                filterContext.Result = redic;
                return;

            }

            base.OnActionExecuting(filterContext);
        }
    }
}