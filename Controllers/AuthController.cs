using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using TM.Core.Helper;

namespace Billing.Core.Controllers {
    public class AuthController : BaseController {
        public ActionResult Index() {
            try {
                getConnection();
            } catch (Exception) {
                return Json(new { danger = "Connection to database fail, Please try again!" });
            }
            return View();
        }

        [HttpGet]
        public JsonResult CheckConnection(AuthObj obj) {
            try {
                getConnection();
            } catch (Exception) {
                return Json(new { danger = "Connection to database fail, Please try again!" });
            }
            return Json(new { success = "Đăng nhập thành công!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Login(AuthObj obj) {
            try {
                var Con = new TM.Core.Connection.Oracle("ORA_PORTAL");
                var qry = "";
                //var collection = HttpContext.Request.ReadFormAsync();
                //string username = collection["username"].ToString();
                //string password = collection["password"].ToString();

                //AuthStatic
                var AuthStatic = Authentication.Core.Auth.isAuthStatic(obj.username, obj.password);
                if (AuthStatic != null) {
                    Authentication.Core.Auth.SetAuth(AuthStatic);
                    return Json(new { success = "Đăng nhập thành công!", url = TM.Core.Helper.Url.RedirectContinue() });
                }
                //AuthDB
                qry = $"SELECT * FROM users WHERE username='{obj.username}'";
                var user = Con.Connection.QueryFirstOrDefault<Authentication.Core.Users>(qry); // db.users.SingleOrDefault(u => u.username == username);

                //Account not Exist
                if (user == null) {
                    return Json(new { danger = "Sai tên tài khoản hoặc mật khẩu!" });
                }

                //Password wrong
                obj.password = TM.Core.Encrypt.MD5.CryptoMD5TM(obj.password + user.salt);
                if (user.password != obj.password) {
                    return Json(new { danger = "Sai tên tài khoản hoặc mật khẩu!" });
                }

                //Account is locked
                if (user.flag < 1) {
                    return Json(new { danger = "Tài khoản đã bị khóa. Vui lòng liên hệ admin!" });
                }

                //Update last login
                user.lastlogin = DateTime.Now;
                Con.Connection.Update(user);
                //Set Auth Account
                Authentication.Core.Auth.SetAuth(user, AuthRoles(user), AuthAllowRoles());
                var b = Authentication.Core.Auth.AuthUser;
                var c = Authentication.Core.Auth.isAuth;
                //return Redirect(TM.Url.RedirectContinue());
            } catch (Exception) {
                return Json(new { danger = "Đăng nhập không thành công, vui lòng liên hệ admin!" });
            }
            return Json(new { success = "Đăng nhập thành công!", url = TM.Core.Helper.Url.RedirectContinue(obj.currentUrl, true) });
        }

        [HttpGet, MiddlewareFilters.Auth]
        public ActionResult logout() {
            try {
                Authentication.Core.Auth.Logout();
            } catch (Exception) {
                return Json(new { danger = "Lỗi vui lòng thử lại sau!" });
            }
            return Json(new { success = "Đăng xuất hệ thống thành công!", url = TM.Core.HttpContext.Current.Http.Request.Headers["Referer"].ToString() });
        }

        [MiddlewareFilters.Auth]
        public ActionResult ChangePassword(Guid id) {
            //return View(db.users.SingleOrDefault(u => u.id == id));
            return View();
        }

        [HttpPost, MiddlewareFilters.Auth]
        public JsonResult ChangePassword(Guid id, string password) {
            try {
                var qry = $"SELECT * FROM users WHERE id='{id.ToString()}'";
                var user = _Con.Connection.QueryFirstOrDefault<Authentication.Core.Users>(qry);
                user.password = TM.Core.Encrypt.MD5.CryptoMD5TM(password + user.salt);
                _Con.Connection.Update(user);
            } catch (Exception) {
                return Json(new { danger = "Cập nhật mật khẩu không thành công, vui lòng thử lại sau!" });
            }
            return Json(new { success = "Cập nhật mật khẩu thành công!" });
        }
        private List<Authentication.Core.RolesAcess> AuthAllowRoles() {
            var rs = new List<Authentication.Core.RolesAcess>() {
                new Authentication.Core.RolesAcess { Controller = "Auth".ToLower(), Action = "*" },
                new Authentication.Core.RolesAcess { Controller = "*", Action = "Index".ToLower() },
                new Authentication.Core.RolesAcess { Controller = "*", Action = "DataExistsCheck".ToLower() },
                //new TMShopsCore.Models.RolesAcess { Controller = "*", Action = "PartialCreate".ToLower() },
                //new TMShopsCore.Models.RolesAcess { Controller = "*", Action = "PartialEdit".ToLower() }
            };
            return rs;
        }
        private List<Authentication.Core.RolesAcess> AuthRoles(Authentication.Core.Users user) {
            var rs = new List<Authentication.Core.RolesAcess>();
            var roles = user.roles.Trim().Trim(',').Split(',');
            foreach (var i in roles)
                rs.Add(new Authentication.Core.RolesAcess() { Controller = i, Action = "*" });
            return rs;
        }
        // private List<Authentication.Core.RolesAcess> AuthRoles(string userRoles) {
        //     var rs = new List<Authentication.Core.RolesAcess>();
        //     var roles = db.Roles.Where(m => userRoles.Contains("," + m.Id.ToString() + ",")).ToList();
        //     foreach (var item in roles) {
        //         var tmp = item.Modules.Trim(',').Split(',');
        //         foreach (var i in tmp) {
        //             var tmp2 = i.Split('.');
        //             if (tmp2.Length > 1 && !rs.Any(m => m.Controller == tmp2[0] && m.Action == tmp2[1]))
        //                 rs.Add(new Authentication.Core.RolesAcess() { Controller = tmp2[0], Action = tmp2[1] });
        //         }
        //     }
        //     return rs;
        // }
    }
    public class AuthObj {
        public string username { get; set; }
        public string password { get; set; }
        public bool rememberMe { get; set; }
        public string currentUrl { get; set; }
    }
}