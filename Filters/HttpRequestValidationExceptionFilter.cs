using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Filters;
using Orchard.Themes;

namespace HttpRequestValidationExceptionFilter.Filters
{
    public class HttpRequestValidationExceptionFilter : FilterProvider, IExceptionFilter
    {
        public ILogger Logger { get; set; }

        public HttpRequestValidationExceptionFilter()
        {
            Logger = NullLogger.Instance;            
        }

        public void OnException(ExceptionContext filterContext) {
            if (filterContext.Exception is HttpRequestValidationException) {
                var controller = (string)filterContext.RouteData.Values["controller"];
                var action = (string)filterContext.RouteData.Values["action"];
                if (controller == null)
                    controller = String.Empty;

                if (action == null)
                    action = String.Empty;

                var model = new HandleErrorInfo(filterContext.Exception, controller, action);
                var result = new ViewResult
                {
                    ViewName = "HttpRequestValidation",
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };

                filterContext.HttpContext.Items[typeof(ThemeFilter)] = null;

                filterContext.Result = result;

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
            }
        }
    }
}