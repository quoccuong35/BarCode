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
    [RoutePrefix("api/NhanHang")]
    public class ApiNhanHangController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_NhanHang_DanhSach_Result>> DanhSach(int NguoiDung,string TimKiem)
        {
            return await Task.Run(() => db.App_NhanHang_DanhSach(NguoiDung, TimKiem).ToList());
        }
        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_NhanHang_ChiTiet_Result>> ChiTiet(int Id)
        {
            return await Task.Run(() => db.App_NhanHang_ChiTiet(Id).ToList());
        }
        [HttpGet]
        [Route("ChiTietKe")]
        public async Task<List<App_NhanHang_ChiTiet_Ke_Result>> ChiTietKe(int Id)
        {
            return await Task.Run(() => db.App_NhanHang_ChiTiet_Ke(Id).ToList());
        }
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, int IdPhieuNhapHang, string SoPhieu, string QRCode,string KyHieu)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                string[] qr = QRCode.Replace(KyHieu,"").Split('_');
                if (qr.Length==2)
                {
                    int poct = int.Parse(qr[0]), num = int.Parse(qr[1]);
                    var model = await Task.Run(() => db.App_NhanHang_UpdateQRCode(NguoiDung, IdPhieuNhapHang, SoPhieu,  poct,num,QRCode).FirstOrDefault());
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
        [HttpGet]
        [Route("QuetThongTinQR1")]
        public async Task<List<app_QuetThongTinQR1_Result>> QuetThongTinQR1(string QRCode,string Loai)
        {
            return await Task.Run(() => db.app_QuetThongTinQR1(QRCode,Loai).ToList());
        }
    }
}
