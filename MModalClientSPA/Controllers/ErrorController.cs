using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MModalClientSPA.ErrorHandler.Exceptions;

namespace MModalClientSPA.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public void LogJavaScriptError(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new JavaScriptException(message));             
        }

        public void LogHostError(Error error)
        {            
            ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(error);
        }

        public void LogSpaError(Error error)
        {
            ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(error);
        }
    }
}