using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Core.Controllers
{
    [MiddlewareFilters.Auth(Role = Authentication.Core.Roles.superadmin + "," + Authentication.Core.Roles.admin + "," + Authentication.Core.Roles.managerBill)]
    public class GuideController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}