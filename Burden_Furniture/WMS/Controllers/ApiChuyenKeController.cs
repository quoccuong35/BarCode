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
    [RoutePrefix("api/ChuyenKe")]
    public class ApiChuyenKeController : ApiController
    {
        WMSEntities db = new WMSEntities();

        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_ChuyenKe_ChiTiet_Result>> ChiTiet(int Id)
        {
            return await Task.Run(() => db.App_ChuyenKe_ChiTiet(Id).ToList());
        }
        [HttpGet]
        [Route("ChiTietKe")]
        public async Task<List<App_ChuyenKe_ChiTiet_Ke_Result>> ChiTietKe( int IdKe)
        {
            return await Task.Run(() => db.App_ChuyenKe_ChiTiet_Ke(IdKe).ToList());
        }
        [HttpGet]
        [Route("KiemTraMa")]
        public async Task<JsonStatus> KiemTraMa( string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                string[] qr = QRCode.Split('_');
                if (qr.Length == 2) { 
                    var model = await db.BD_PhieuNhapHangChiTiet.FirstOrDefaultAsync(m => m.SoEPI == null && m.QRCode == QRCode);
                    if (model != null)
                    {
                        st.code = (int)model.IDPhieuNhapHangChiTiet;
                        st.text = model.MaChiTietSanPham + " " + model.TenChiTietSanPham;
                    }
                    else
                    {
                        st.code = 0;
                        st.text = "Không tồn tại hàng cần chuyển trong hệ thống.";
                    }
                }
                else
                {
                    st.code = 0;
                    st.text = "QRCode không hợp lệ.";
                }
            }
            catch
            {
                st.code = 0;
                st.text = "Đã có lỗi xảy ra, xin vui lòng thử lại.";
            }
            return st;
        }
        [HttpPost]
        [Route("ChuyenKe")]
        public  async Task<JsonStatus> ChuyenKe(int NguoiDung,int IDPhieuNhapHangChiTiet,  int IdKe,string MaKe, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var chitiet = await db.BD_PhieuNhapHangChiTiet.FirstOrDefaultAsync(m => m.QRCode == QRCode);
                if (IdKe == chitiet.IdKe)
                {
                    st.code = 0;
                    st.text = "Bạn không thể chuyển hàng trên cùng 1 kệ.";
                }
                else
                {
                    BD_LichSuChuyenKe lichsu = new BD_LichSuChuyenKe();
                    lichsu.IDPhieuNhapHangChiTiet = IDPhieuNhapHangChiTiet;
                    lichsu.IdKeCu = (int)chitiet.IdKe;
                    lichsu.IdKeMoi = IdKe;
                    lichsu.NgayChuyen = DateTime.Now;
                    lichsu.NguoiDungChuyen = NguoiDung;
                    lichsu.QRCode = QRCode;
                    lichsu.SoLuong = 1;
                    chitiet.IdKe = IdKe;
                    chitiet.MaKe = MaKe;
                    db.BD_LichSuChuyenKe.Add(lichsu);
                    db.SaveChanges();

                    st.code = (int)lichsu.ID;
                    st.text = "Chuyển kệ thành công";
                }
            }
            catch(Exception ex)
            {
                st.code = 0;
                st.text = "Đã có lỗi xảy ra, xin vui lòng thử lại.";
            }
            return st;
        }
    }
}
