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
    public class ChuyenKeApiController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_ChuyenKe_DanhSach_Result>> DanhSach(int NguoiDung,string TimKiem)
        {
            return await Task.Run(() => db.App_ChuyenKe_DanhSach(NguoiDung, TimKiem).ToList());
        }
        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_ChuyenKe_ChiTiet_Result>> ChiTiet(int Id)
        {
            return await Task.Run(() => db.App_ChuyenKe_ChiTiet(Id).ToList());
        }
        [HttpGet]
        [Route("ChiTietKe")]
        public async Task<List<App_ChuyenKe_ChiTiet_Ke_Result>> ChiTietKe(int IdPhieuNhapHang, int IdKe1,int IdKe2)
        {
            return await Task.Run(() => db.App_ChuyenKe_ChiTiet_Ke(IdPhieuNhapHang, IdKe1, IdKe2).ToList());
        }
        [HttpGet]
        [Route("KiemTraMa")]
        public async Task<JsonStatus> KiemTraMa(int IdPhieuNhapHang, int IdKe, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                if (QRCode.All(char.IsDigit))
                {
                    int poct = int.Parse(QRCode);
                    var model = await db.BD_PhieuNhapHangChiTiet.FirstOrDefaultAsync(m => m.IdPhieuNhapHang == IdPhieuNhapHang && m.IdKe == IdKe && m.QRCode == QRCode);
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
        public  async Task<JsonStatus> ChuyenKe(int NguoiDung,int IDPhieuNhapHangChiTiet, int IdKe1, int IdKe2,string MaKe2, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                BD_LichSuChuyenKe lichsu = new BD_LichSuChuyenKe();
                lichsu.IDPhieuNhapHangChiTiet = IDPhieuNhapHangChiTiet;
                lichsu.IdKeCu = IdKe1;
                lichsu.IdKeMoi = IdKe2;
                lichsu.NgayChuyen = DateTime.Now;
                lichsu.NguoiDungChuyen = NguoiDung;
                lichsu.QRCode = QRCode;
                lichsu.SoLuong = 1;
                db.BD_LichSuChuyenKe.Add(lichsu);
                var chitiet = await db.BD_PhieuNhapHangChiTiet.FirstOrDefaultAsync(m => m.IDPhieuNhapHangChiTiet == IDPhieuNhapHangChiTiet);
                chitiet.IdKe = IdKe2;
                chitiet.MaKe = MaKe2;
                await db.SaveChangesAsync();
               
                    st.code = (int)lichsu.ID;
                st.text = "Chuyển kệ thành công";
            }
            catch
            {
                st.code = 0;
                st.text = "Đã có lỗi xảy ra, xin vui lòng thử lại.";
            }
            return st;
        }
    }
}
