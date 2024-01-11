using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using WMS.DB;
using WMS.Models;
using System.Data.Entity;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("api/NhapKhoThanhPham")]
    public class ApiNhapKhoThanhPhamController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_NhapThanhPham_Result>> DanhSach(int NguoiDung)
        {
            return await Task.Run(() => db.App_NhapThanhPham(NguoiDung).ToList());
        }
        [HttpPost]
        [Route("QuetNhapKho")]
        public async Task<JsonStatus> QuetNhapKho(int NguoiDung,int IdKe,string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var arr = QRCode.Split('_');
                if (arr.Length == 2)
                {
                    int IdLenh = int.Parse(arr[0]), NumOf = int.Parse(arr[1]);
                    var model = await Task.Run(() => db.App_NhapThanhPham_UpdateQRCode(NguoiDung, IdKe, IdLenh,NumOf,QRCode).FirstOrDefault());
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
