using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Billing.Core.MiddlewareFilters
{
    public class Auth : ActionFilterAttribute
    {
        public string Role { get; set; }
        public Auth() { }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executed
            // var result = new RedirectToRouteResult(
            //     new Microsoft.AspNetCore.Routing.RouteValueDictionary {
            //         { "controller", "Auth" },
            //         { "action", "Index" },
            //         { "continue", TM.Core.Helper.Url.Continue } });
            // var authController = result.RouteValues["controller"].ToString().ToUpper();
            // var actionController = result.RouteValues["action"].ToString().ToUpper();
            // var currentController = context.RouteData.Values["controller"].ToString().ToUpper().Replace("API", "");
            // var currentAction = context.RouteData.Values["action"].ToString().ToUpper();
            // if (Common.Auth.isAuth) //Logged
            // {
            //     var roles = Common.Auth.AuthRoles;
            //     var AllowRoles = Common.Auth.AuthAllowRoles;
            //     if (Common.Auth.AuthAllowRoles.Any(m => m.Controller == "*" && m.Action == currentAction))
            //     {
            //         //if (!Common.Auth.Roles.Any(m => m.Action == currentAction))
            //         //    context.Result = result;
            //     }
            //     else if (Common.Auth.AuthAllowRoles.Any(m => m.Controller == currentController && m.Action == "*"))
            //     {
            //         //if (!Common.Auth.Roles.Any(m => m.Controller == currentController))
            //         //    context.Result = result;
            //     }
            //     else if (Common.Auth.AuthAllowRoles.Any(m => m.Controller == currentController && m.Action == currentAction))
            //     {
            //         //if (!Common.Auth.Roles.Any(m => m.Controller == currentController))
            //         //    context.Result = result;
            //     }
            //     else
            //         if (!Common.Auth.AuthRoles.Any(m => m.Controller == currentController && m.Action == currentAction))
            //         context.Result = result;
            // }
            // else //Not Logged
            // {
            //     if (currentController != authController || currentAction != actionController)
            //     {
            //         context.Result = result;
            //     }
            //     else
            //     {

            //     }
            // }
            base.OnActionExecuting(context);
        }
    }
    public class AuthAsync : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // do something before the action executes
            await next();
            // do something after the action executes
        }
    }
}