using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Billing.Core.MiddlewareFilters {
    public class Auth : ActionFilterAttribute {
        public string Role { get; set; }
        public Auth() { }
        public override void OnActionExecuting(ActionExecutingContext context) {
            // do something before the action executed
            var result = new RedirectToRouteResult(
                new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "controller", "Auth" }, { "action", "Index" }, { "continue", TM.Core.Helper.Url.Continue } });
            var authController = result.RouteValues["controller"].ToString().ToLower();
            var actionController = result.RouteValues["action"].ToString().ToLower();
            var currentController = context.RouteData.Values["controller"].ToString().ToLower().Replace("api", "");
            var currentAction = context.RouteData.Values["action"].ToString().ToLower();
            if (Authentication.Core.Auth.isAuth) //Logged
            {
                var roles = Authentication.Core.Auth.AuthRoles;
                var AllowRoles = Authentication.Core.Auth.AuthAllowRoles;
                if (Authentication.Core.Auth.AuthAllowRoles.Any(m => m.Controller == "*" && m.Action == currentAction)) {
                    //if (!Common.Auth.Roles.Any(m => m.Action == currentAction))
                    //    context.Result = result;
                } else if (Authentication.Core.Auth.AuthAllowRoles.Any(m => m.Controller == currentController && m.Action == "*")) {
                    //if (!Common.Auth.Roles.Any(m => m.Controller == currentController))
                    //    context.Result = result;
                } else if (Authentication.Core.Auth.AuthAllowRoles.Any(m => m.Controller == currentController && m.Action == currentAction)) {
                    //if (!Common.Auth.Roles.Any(m => m.Controller == currentController))
                    //    context.Result = result;
                } else if (Authentication.Core.Auth.AuthRoles.Any(m => m.Controller == currentController && m.Action == "*")) {

                } else if (!Authentication.Core.Auth.AuthRoles.Any(m => m.Controller == currentController && m.Action == currentAction))
                    context.Result = result;
            } else //Not Logged
            {
                if (currentController != authController) {
                    context.Result = result;
                }
                // else if (currentAction != actionController) {
                //     context.Result = result;
                // } 
                else {

                }
            }
            base.OnActionExecuting(context);
        }
    }
    public class AuthAsync : IAsyncActionFilter {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next) {
            // do something before the action executes
            await next();
            // do something after the action executes
        }
    }
}