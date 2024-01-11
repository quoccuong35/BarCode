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
    [RoutePrefix("api/SanXuat")]
    public class ApiSanXuatController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_SanXuat_DanhSach_Result>> DanhSach(string TimKiem)
        {
            return await Task.Run(() => db.App_SanXuat_DanhSach(TimKiem).ToList());
        }
        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_SanXuat_DanhSach_ChiTiet_Result>> ChiTiet(string MaCongDoan)
        {
            return await Task.Run(() => db.App_SanXuat_DanhSach_ChiTiet(MaCongDoan).ToList());
        }
        [HttpGet]
        [Route("ChiTietTru")]
        public async Task<List<App_SanXuat_DanhSach_ChiTietTru_Result>> ChiTietTru(string MaCongDoan)
        {
            return await Task.Run(() => db.App_SanXuat_DanhSach_ChiTietTru( MaCongDoan).ToList());
        }
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, string MaCongDoan, int SoThuTu,  string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length==2){
                    int Id = int.Parse(arr[0]); int Num = int.Parse(arr[1]);
                    var model = await Task.Run(() => db.App_SanXuat_UpdateQrCode(NguoiDung, MaCongDoan, SoThuTu, Id, Num,QRCode).FirstOrDefault());
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
        [Route("KiemTraCongDoan")]
        public async Task<App_SanXuat_KTCongDoan_Result> KiemTraCongDoan( string QRCode)
        {
            return await Task.Run(() => db.App_SanXuat_KTCongDoan(0, QRCode).FirstOrDefault());
        }
        [HttpPost]
        [Route("Tru")]
        public async Task<JsonStatus> Tru(int NguoiDung,string SoEPI, string MaCongDoan,string LyDo,  string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length == 2)
                {
                    int Id = int.Parse(arr[0]); int Num = int.Parse(arr[1]);
                    var model = await Task.Run(() => db.App_SanXuat_UpdateQrCodeTruCD(NguoiDung, SoEPI, MaCongDoan, LyDo, QRCode).FirstOrDefault());
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

        [HttpPost]
        [Route("QuetMaNhanh")]
        public async Task<JsonStatus> QuetMaNhanh(int NguoiDung, string MaCongDoan, int SoThuTu, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length == 2)
                {
                    int Id = int.Parse(arr[0]); int Num = int.Parse(arr[1]);
                    var model = await Task.Run(() => db.app_SanXuat_UpdateQrCodeNhanh(NguoiDung, MaCongDoan, SoThuTu, Id, Num, QRCode).FirstOrDefault());
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
