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
    [RoutePrefix("api/XuatHangTrang")]
    public class ApiXuatHangTrangController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_XuatHangTrang_Result>> DanhSach(int Id)
        {
            return await Task.Run(() => db.App_XuatHangTrang(Id).ToList());
        }
        [HttpGet]
        [Route("Ke")]
        public async Task<List<App_XuatHangTrang_Ke_Result>> Ke(string SoEPI)
        {
            return await Task.Run(() => db.App_XuatHangTrang_Ke(SoEPI).ToList());
        }
        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_XuatHangTrang_ChiTiet_Result>> ChiTiet(string SoEPI)
        {
            return await Task.Run(() => db.App_XuatHangTrang_ChiTiet(SoEPI).ToList());
        }
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, string SoEPI,string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                string[] qr = QRCode.Split('_');
                if (qr.Length == 2)
                {
                    var model = await Task.Run(() => db.App_XuatHangTrang_UpdateQRCode(NguoiDung, SoEPI,  QRCode).FirstOrDefault());
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
        [Route("ChiTietKhac")]
        public async Task<List<App_XuatHangTrang_ChiTietKhac_Result>> ChiTietKhac(int NguoiDung)
        {
            return await Task.Run(() => db.App_XuatHangTrang_ChiTietKhac(NguoiDung).ToList());
        }
        [HttpPost]
        [Route("QuetMaKhac")]
        public async Task<JsonStatus> QuetMaKhac(int NguoiDung, string GhiChu, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                string[] qr = QRCode.Split('_');
                if (qr.Length == 2)
                {
                    var model = await Task.Run(() => db.App_XuatHangTrang_UpdateQRCodeKhac(NguoiDung, GhiChu, QRCode).FirstOrDefault());
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
