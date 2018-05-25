using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Core.Controllers {
    [MiddlewareFilters.Auth]
    public class ImportData : BaseController {
        public IActionResult Index() {
            // var oracle = new TM.Core.Connection.Oracle("VNPTBK");
            // ViewBag.data = oracle.Connection.Query("SELECT * FROM J_PURCHASEORDER");
            return View();
        }
        public JsonResult GetTable(string database = "SQL_CUOC") {
            try {
                var SQLServer = new TM.Core.Connection.SQLServer(database);
                var qry = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME";
                var data = SQLServer.Connection.Query<Models.TABLES>(qry).ToList();
                return Json(new { data = data, success = "Xử lý thành công!" });
            } catch (System.Exception ex) {
                return Json(new { danger = ex.Message });
            }
        }
        public JsonResult TransferData(string dataVal, string database = "SQL_CUOC") {
            var SQLServer = new TM.Core.Connection.SQLServer(database);
            var qry = $"SELECT * FROM {dataVal}";
            var tableOld = SQLServer.Connection.Query(qry);

            var tableNew = _Con.Connection.Query(qry).ToList();

            return Json("");
        }
        public JsonResult TransferDataPortal(string dataVal, string database = "SQL_Portal") {
            try {
                var SQLServer = new TM.Core.Connection.SQLServer(database);
                var Oracle = new TM.Core.Connection.Oracle("ORA_PORTAL");
                var qry = $"SELECT * FROM {dataVal}";
                var table = SQLServer.Connection.Query<Authentication.Core.Users>(qry);
                foreach (var i in table) {
                    i.UserID = i.UserID.Replace("-", "").ToUpper();
                    i.fullname = string.IsNullOrEmpty(i.fullname) ? i.username : i.fullname;
                    i.createdBy = string.IsNullOrEmpty(i.createdBy) ? "Admin" : i.createdBy;
                    i.createdAt = i.createdAt.HasValue ? i.createdAt.Value : DateTime.Now;
                }
                Oracle.Connection.Insert(table);

                return Json(new { success = "Cập nhật thành công" });
            } catch (System.Exception ex) {
                return Json(new { danger = ex.Message });
            }
        }
    }
}