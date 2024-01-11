using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.DB;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.Controllers
{
    public class HangTrangController : Controller
    {
        // GET: HangTrang
        WMSEntities db = new WMSEntities();
        public ActionResult Index()
        {
            return View();
        }
        public async Task<JsonResult> getDataLichSuKeHangTrang(DateTime TuNgay, DateTime DenNgay, String MaKe, string SoEPI, string MaSKU, string MaSP)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                db.Database.CommandTimeout = 3600;
                var model = db.BD_LichSuXuatHangTrang(TuNgay, DenNgay, MaKe, SoEPI, MaSKU, MaSP).ToList();
                rs.data = model;
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.code = 0;
                rs.description = ex.Message;
            }
            var json = Json(rs, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
    }
}