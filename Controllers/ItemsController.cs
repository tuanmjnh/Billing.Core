﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Core.Controllers {
    [MiddlewareFilters.Auth(Role = Authentication.Core.Roles.superadmin + "," + Authentication.Core.Roles.admin + "," + Authentication.Core.Roles.managerBill)]
    public class ItemsController : BaseController {
        public ActionResult Index() {
            return View();
        }
        public ActionResult Insert() {
            return PartialView("PartialCreate");
        }
        public ActionResult Update() {
            return PartialView("PartialEdit");
        }

        [HttpGet]
        public ActionResult Select(objBST obj) //string sort, string order, string search, int offset = 0, int limit = 10, int flag = 1
        {
            var index = 0;
            var qry = "";
            var cdt = "";
            try {
                qry = $"SELECT i.*,g.TITLE AS GROUPTITLE FROM ITEMS i,GROUPS g WHERE i.GROUPID=g.GROUPID AND i.FLAG={obj.flag} AND g.FLAG=1";

                //Get data for Search
                if (!string.IsNullOrEmpty(obj.search))
                    cdt += $"(i.TITLE LIKE '%{obj.search}%' OR g.TITLE LIKE '%{obj.search}%') AND ";
                if (!string.IsNullOrEmpty(obj.groupID) && obj.groupID != "0")
                    cdt += $"(i.GROUPID='{obj.groupID}') AND ";
                if (!string.IsNullOrEmpty(cdt))
                    qry += $" AND {cdt.Substring(0, cdt.Length - 5)}";

                //export
                if (obj.export == 1) {
                    //var startDate = DateTime.ParseExact($"{obj.startDate}", "dd/MM/yyyy HH:mm", provider);
                    //var endDate = DateTime.ParseExact($"{obj.endDate}", "dd/MM/yyyy HH:mm", provider);
                    //qry += $" AND tb.FLAG=2 AND tb.UPDATEDAT>=CAST('{startDate.ToString("yyyy-MM-dd")}' as datetime) AND tb.UPDATEDAT<=CAST('{endDate.ToString("yyyy-MM-dd")}' as datetime) ORDER BY tb.MA_DVI,tb.UPDATEDAT";
                    //var export = _Con.Connection.Query<Portal.Areas.ND49.Models.ND49Export>(qry);
                    //qry = "SELECT * FROM users";
                    //var user = _Con.Connection.Query<Authentication.user>(qry);
                    //foreach (var i in export)
                    //{
                    //    var tmp = user.FirstOrDefault(d => d.username == i.NVQL);
                    //    i.TEN_NVQL = tmp != null ? tmp.full_name : null;
                    //}
                    //var rsJson = Json(new { data = export, SHA = Guid.NewGuid() });
                    //rsJson.MaxJsonLength = int.MaxValue;
                    //return rsJson;
                }
                //
                var data = _Con.Connection.Query<Models.ITEMS_G>(qry);

                ////Get data for Search
                //if (!string.IsNullOrEmpty(obj.search))
                //    data = data.Where(d =>
                //    d.MA_KH.Contains(obj.search) ||
                //    d.MA_TT_HNI.Contains(obj.search) ||
                //    d.ACCOUNT.Contains(obj.search) ||
                //    d.MA_TB.Contains(obj.search) ||
                //    d.TEN_TT.Contains(obj.search) ||
                //    d.DIACHI_TT.Contains(obj.search));
                //
                if (data.ToList().Count < 1)
                    return Json(new { total = 0, rows = data });
                //Get total item
                var total = data.Count();
                //Sort And Orders
                if (!string.IsNullOrEmpty(obj.sort)) {
                    if (obj.sort.ToUpper() == "TITLE" && obj.order.ToLower() == "asc")
                        data = data.OrderBy(m => m.TITLE);
                    else if (obj.sort.ToUpper() == "TITLE" && obj.order.ToLower() == "desc")
                        data = data.OrderByDescending(m => m.TITLE);
                    else
                        data = data.OrderBy(m => m.ORDERS).ThenBy(m => m.GROUPID).ThenBy(m => m.TITLE);
                } else
                    data = data.OrderBy(m => m.ORDERS).ThenBy(m => m.GROUPID).ThenBy(m => m.TITLE);
                //Page Site
                var rs = data.Skip(obj.offset).Take(obj.limit).ToList();
                var ReturnJson = Json(new { total = total, rows = rs });
                //ReturnJson.MaxJsonLength = int.MaxValue;
                return ReturnJson;
            } catch (Exception) { return Json(new { danger = "Không tìm thấy dữ liệu, vui lòng thực hiện lại!" }); } 
            //return Json(new { success = "Cập nhật thành công!" });
        }

        [HttpGet]
        public ActionResult GetGroups() {
            var index = 0;
            var qry = "";
            try {
                qry = $"SELECT * FROM GROUPS WHERE FLAG=1 ORDER BY ORDERS";
                var data = _Con.Connection.Query<Models.GROUPS>(qry);
                var ReturnJson = Json(new { data = data, success = "Lấy dữ liệu thành công!" });
                //ReturnJson.MaxJsonLength = int.MaxValue;
                return ReturnJson;
            } catch (Exception) { return Json(new { danger = "Không tìm thấy dữ liệu, vui lòng thực hiện lại!" }); } 
        }

        [HttpGet]
        public ActionResult Get(long id) {
            var index = 0;
            var qry = "";
            try {
                qry = $"SELECT * FROM ITEMS WHERE ITEMID={id}";
                var data = _Con.Connection.QueryFirstOrDefault<Models.ITEMS>(qry);
                qry = $"SELECT * FROM GROUPS WHERE FLAG=1 ORDER BY ORDERS";
                var groups = _Con.Connection.QueryFirstOrDefault<Models.GROUPS>(qry);
                var ReturnJson = Json(new { data = data, groups = groups, success = "Lấy dữ liệu thành công!" });
                //ReturnJson.MaxJsonLength = int.MaxValue;
                return ReturnJson;
            } catch (Exception) { return Json(new { danger = "Không tìm thấy dữ liệu, vui lòng thực hiện lại!" }); } 
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InsertUpdate(Models.ITEMS obj, long? id) //string sort, string order, string search, int offset = 0, int limit = 10, int flag = 1
        {
            //var provider = System.Globalization.CultureInfo.InvariantCulture;
            var index = 0;
            var qry = "";
            var msg = "Cập nhật thông tin thành công!";
            try {
                //
                if (id == null) {
                    obj.CREATEDBY = Authentication.Core.Auth.AuthUser.username;
                    obj.CREATEDAT = DateTime.Now;
                    _Con.Connection.Insert(obj);
                    msg = "Cập nhật thông tin thành công!";
                } else {
                    qry = $"SELECT * FROM ITEMS WHERE ITEMID={id}";
                    var data = _Con.Connection.QueryFirstOrDefault<Models.ITEMS>(qry);
                    data.APPKEY = obj.APPKEY;
                    data.GROUPID = obj.GROUPID;
                    data.TITLE = obj.TITLE;
                    data.DESCRIPTION = obj.DESCRIPTION;
                    data.CONTENTS = obj.CONTENTS;
                    data.AMOUNT_START = obj.AMOUNT_START;
                    data.AMOUNT_END = obj.AMOUNT_END;
                    data.IMAGES = obj.IMAGES;
                    data.ATTACH = obj.ATTACH;
                    data.AUTHOR = obj.AUTHOR;
                    data.QUANTITY = obj.QUANTITY;
                    data.POSITION = obj.POSITION;
                    data.ORDERS = obj.ORDERS;
                    data.FLAG = obj.FLAG;
                    data.UPDATEDBY = Authentication.Core.Auth.AuthUser.username;
                    data.UPDATEDAT = DateTime.Now;
                    _Con.Connection.Update(data);
                }
                var ReturnJson = Json(new { success = msg });
                //ReturnJson.MaxJsonLength = int.MaxValue;
                return ReturnJson;
            } catch (Exception) { return Json(new { danger = "Lỗi hệ thống vui lòng thực hiện lại!" }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(string id) {
            var index = 0;
            var qry = "";
            var msg = "Xóa bản ghi thành công!";
            try {
                var _id = id.Trim(',');

                qry = $"SELECT * FROM ITEMS WHERE ITEMID IN({_id})";
                var data = _Con.Connection.QueryFirstOrDefault<Models.ITEMS>(qry);

                if (data == null) return Json(new { danger = "Không tìm thấy dữ liệu!" });
                if (data.FLAG == 0) {
                    qry = $"UPDATE ITEMS SET FLAG=1 WHERE ITEMID IN({_id})";
                    msg = "Khôi phục bản ghi thành công!";
                } else
                    qry = $"UPDATE ITEMS SET FLAG=0,DELETEDBY='{Authentication.Core.Auth.AuthUser.username}',DELETEDAT=GETDATE() WHERE ITEMID IN({_id})";
                _Con.Connection.Query(qry);

                var ReturnJson = Json(new { success = msg });
                //ReturnJson.MaxJsonLength = int.MaxValue;
                return ReturnJson;
            } catch (Exception) { return Json(new { danger = "Lỗi hệ thống vui lòng thực hiện lại!" }); } finally { _Con.Close(); }
        }
        public class objBST : Common.ObjBSTable {
            public string groupID { get; set; }
            public string timeBill { get; set; }
            public int export { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
        }
    }
}