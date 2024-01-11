using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using WMS.DB;
using WMS.Models;
namespace WMS.Controllers
{
    [RoleAuthorize]
    public class DataController : Controller
    {
        WMSEntities db = new WMSEntities();
        public async Task<JsonResult> GetNhaCungCap()
        {
            return Json(await db.HS_NhaCungCap.Where(m=>m.HienThi==true).ToListAsync(),JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetKeHang()
        {
            return Json(await db.HS_Ke.ToListAsync(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetNguoiDung()
        {
            return Json(await db.HT_NguoiDung.Where(it=>it.IdNguoiDung>0 && it.HoatDong == true).ToListAsync(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetNhomNguoiDung()
        {
            return Json(await db.HT_NhomNguoiDung.Where(it=>it.IdNhom>0 && it.Xoa !=true).ToListAsync(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetCongDoan()
        {
            return Json(await db.HS_CongDoan.ToListAsync(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetEPI()
        {
            return Json(await db.BD_LenhRaiHang.Where(it=>it.Del !=true).Select(it=>new {SoEPI = it.SoEPI,Ten = it.SoEPI}).Distinct().ToListAsync(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetKeHangTrang()
        {
            var model = await(from a in db.BD_PhieuNhapHangChiTiet
                         join b in db.HS_Ke on a.IdKe equals b.IdKe
                         where a.XuatKho != true
                         select b).Distinct().ToListAsync();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetKeThanhPham()
        {
            var model = await (from a in db.BD_NhapKhoThanhPham
                               join b in db.HS_Ke on a.IdKe equals b.IdKe
                               where a.XuatKho != true
                               select b).Distinct().ToListAsync();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}