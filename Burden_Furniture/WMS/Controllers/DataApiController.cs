using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WMS.DB;
using WMS.Models;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("api/Data")]
    public class DataApiController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("UserInfo")]
        public async Task<Helpers.NguoiDung> getUserInfo(string Username)
        {
            var nd = await Task.Run(() => db.PR_ThongTinNguoiDung(Username).FirstOrDefault());
            var ng = new Helpers.NguoiDung();
            ng.IdNguoiDung = nd.IdNguoiDung;
            ng.IdNhom = nd.IdNhom;
            ng.TaiKhoan = nd.TaiKhoan;
            ng.HoTen = nd.HoTen;
            ng.TenNhom = nd.TenNhom;
            return ng;
        }
        [HttpGet]
        [Route("Ke")]
        public async Task<List<HS_Ke>> Ke()
        {
           return await db.HS_Ke.ToListAsync();
        }
        [HttpGet]
        [Route("KeNotIn")]
        public async Task<List<HS_Ke>> KeNotIn(int id)
        {
            return await db.HS_Ke.Where(m=>m.IdKe!=id).ToListAsync();
        }
        [HttpGet]
        [Route("KeTheoPhieu")]
        public async Task<List<HS_Ke>> KeTheoPhieu(int id)
        {
            return await db.Database.SqlQuery<HS_Ke>("SELECT DISTINCT HS_Ke.* FROM dbo.HS_Ke INNER JOIN dbo.BD_PhieuNhapHangChiTiet ON BD_PhieuNhapHangChiTiet.IdKe = HS_Ke.IdKe WHERE IdPhieuNhapHang=" + id).ToListAsync();
        }
        [HttpGet]
        [Route("CauHinh")]
        public async Task<List<Sys_CauHinh>> CauHinh()
        {
            return await db.Sys_CauHinh.ToListAsync();
        }
    }
}