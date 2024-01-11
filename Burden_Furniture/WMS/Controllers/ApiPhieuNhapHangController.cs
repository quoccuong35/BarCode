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
    [RoutePrefix("api/PhieuNhapHang")]
    public class ApiPhieuNhapHangController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_PhieuNhapHang_DanhSach_Result>> DanhSach(int NguoiDung,string TimKiem)
        {
            return await Task.Run(() => db.App_PhieuNhapHang_DanhSach(NguoiDung, TimKiem).ToList());
        }
   
        [HttpGet]
        [Route("ChiTietKe")]
        public async Task<List<App_PhieuNhapHang_ChiTiet_Ke_Result>> ChiTietKe(int Ke)
        {
            return await Task.Run(() => db.App_PhieuNhapHang_ChiTiet_Ke(Ke).ToList());
        }
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, string KyHieu, int IdKe, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                string[] qr = QRCode.Replace(KyHieu, "").Split('_');
                if (qr.Length == 2)
                {
                    var model = await Task.Run(() => db.App_PhieuNhapHang_UpdateQRCode(NguoiDung,  IdKe,QRCode).FirstOrDefault());
                    st.code = (int)model.code;
                    st.text = model.text;
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
    }
}
