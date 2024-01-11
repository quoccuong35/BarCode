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
    public class PhieuNhapHangApiController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_PhieuNhapHang_DanhSach_Result>> DanhSach(int NguoiDung,string TimKiem)
        {
            return await Task.Run(() => db.App_PhieuNhapHang_DanhSach(NguoiDung, TimKiem).ToList());
        }
        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_PhieuNhapHang_ChiTiet_Result>> ChiTiet(int Id)
        {
            return await Task.Run(() => db.App_PhieuNhapHang_ChiTiet(Id).ToList());
        }
        [HttpGet]
        [Route("ChiTietKe")]
        public async Task<List<App_PhieuNhapHang_ChiTiet_Ke_Result>> ChiTietKe(int Id, int Ke)
        {
            return await Task.Run(() => db.App_PhieuNhapHang_ChiTiet_Ke(Id, Ke).ToList());
        }
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, int IdPhieuNhapHang, string SoPhieu, int IdKe, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                if (QRCode.All(char.IsDigit)){
                    int poct = int.Parse(QRCode);
                    var model = await Task.Run(() => db.App_PhieuNhapHang_UpdateQRCode(NguoiDung, IdPhieuNhapHang, SoPhieu, IdKe, poct).FirstOrDefault());
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
