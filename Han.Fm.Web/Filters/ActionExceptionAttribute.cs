using Han.Fm.Model.BaseDto;
using Han.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Han.Fm.Web.Filters
{
    public class ActionExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = new Response<bool>();

            result.ErrMsg = "系统发生错误，请联系管理人员进行反馈。";
            Logger.LogException(context.Exception);
            
            //异常已处理，不需要后续操作
            context.ExceptionHandled = true;

            context.Result = new JsonResult()
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}