using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using Helpers;
using WMS.DB;
using WMS.Models;

namespace WMS.Controllers
{
    [RoleAuthorize]
    public class HomeController : Controller
    {
        WMSEntities db = new WMSEntities();
        public ActionResult GetNguoiDung()
        {
            return Json(db.HT_NguoiDung.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public JsonResult GetDashBoard()
        {
            var model = db.BD_DashboarddBaoCaoTienDoSanXuat().ToList();
            var rs = Json(model, JsonRequestBehavior.AllowGet);
            rs.MaxJsonLength = int.MaxValue;
            return rs;
        }
        public string Pass()
        {
            // return Encode_Decode.Encrypt("abc@123456");
            return Encode_Decode.Decrypt("KHjloaN2u2G/8FStJ8E38Q==");
        }
    }
}