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
    [RoutePrefix("api/XuatContainer")]
    public class ApiXuatContainerController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_XuatContainer_Result>> DanhSach(string SoEPI,int NguoiDung)
        {
            return await Task.Run(() => db.App_XuatContainer(SoEPI, NguoiDung).ToList());
        }
       
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, string SoEPI, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length==2)
                {
                    var model = await Task.Run(() => db.App_XuatContainer_UpdateQRCode(NguoiDung, SoEPI,   QRCode).FirstOrDefault());
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
        [Route("DanhSachSoanHang")]
        public async Task<List<App_SoanHangXuatContainer_Result>> DanhSachSoanHang(string SoEPI,int NguoiDung)
        {
            return await Task.Run(() => db.App_SoanHangXuatContainer(SoEPI,NguoiDung).ToList());
        }
        [HttpPost]
        [Route("SoanHangQuetMa")]
        public async Task<JsonStatus> SoanHangQuetMa(int NguoiDung, string SoEPI, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length == 2)
                {
                    var model = await Task.Run(() => db.App_SoanHangXuatContainer_UpdateQRCode(NguoiDung, SoEPI, QRCode).FirstOrDefault());
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
        [Route("DanhSachTruLenCont")]
        public async Task<List<app_TruLenConDanhSach_Result>> DanhSachTruLenCont( int NguoiDung)
        {
            return await Task.Run(() => db.app_TruLenConDanhSach(NguoiDung).ToList());
        }
        [HttpPost]
        [Route("QuetTruCont")]
        public async Task<JsonStatus> QuetTruCont(int NguoiDung,string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length == 2)
                {
                    var model = await Task.Run(() => Task.Run(() => db.App_TruLenContQRCode(NguoiDung, QRCode).FirstOrDefault()));
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
