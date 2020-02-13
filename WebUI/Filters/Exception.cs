using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Filters
{
    public class IndexException : FilterAttribute, IExceptionFilter
    {

        public void OnException(ExceptionContext exceptionContext)
        {
            if (!exceptionContext.ExceptionHandled && exceptionContext.Exception is ArgumentException)
            {
                exceptionContext.Result = new RedirectResult("/");
                exceptionContext.ExceptionHandled = true;
            }
        }
    }
}