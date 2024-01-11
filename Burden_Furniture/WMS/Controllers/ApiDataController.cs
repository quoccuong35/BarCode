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
    //[Authorize]
    [RoutePrefix("api/Data")]
    public class ApiDataController : ApiController
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
        [HttpGet]
        [Route("EPI")]
        public async Task<List<App_GetEPI_Result>> EPI(int Step)
        {
            return await Task.Run(() => db.App_GetEPI(Step).ToList());
        }
        [HttpGet]
        [Route("CongDoan")]
        public async Task<List<HS_CongDoan>> CongDoan(int NguoiDung)
        {
            List<HS_CongDoan> model = new List<HS_CongDoan>();
            if (NguoiDung > 0)
            {
                //model =await (from nd in db.HT_NguoiDung
                //         join pv in db.HT_PhamViCongDoan on nd.IdNhom equals pv.IdDoiTuong
                //         join cd in db.HS_CongDoan on pv.MaPhamVi equals cd.MaCongDoan
                //         where pv.LaNguoiDung == false && nd.IdNguoiDung == NguoiDung
                //         select cd).ToListAsync();
                var temp = await Task.Run(() => db.App_GetCongDoan(NguoiDung).ToList());
                foreach (var item in temp)
                {
                    model.Add(new HS_CongDoan { MaCongDoan = item.MaCongDoan, TenCongDoan = item.TenCongDoan,
                        HienThi = item.HienThi, STT = item.STT, KiemTra = item.KiemTra, HoanThanh = item.HoanThanh });
                }
            }
            else
            {
                model =await db.HS_CongDoan.ToListAsync();
            }
            if (model.Count == 0)
            {
                model.Add(new HS_CongDoan { MaCongDoan = null, TenCongDoan = "Chưa phân quyền công đoạn" });
            }

            return model;
        }
        [HttpGet]
        [Route("Menu")]
        public async Task<List<App_Menu_Result>> Menu(int NguoiDung)
        {
            return await Task.Run(() => db.App_Menu(NguoiDung).ToList());
        }
    }
}