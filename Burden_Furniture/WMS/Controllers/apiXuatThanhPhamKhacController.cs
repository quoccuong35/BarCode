using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.DB;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("api/XuatThanhPhamKhac")]
    public class apiXuatThanhPhamKhacController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<App_XuatThanhPhamKhac_Result>> DanhSach(int NguoiDung)
        {
            return await Task.Run(() => db.App_XuatThanhPhamKhac(NguoiDung).ToList());
        }
        [HttpPost]
        [Route("QuetMa")]
        public async Task<JsonStatus> QuetMa(int NguoiDung, string GhiChu, string QRCode)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                string[] qr = QRCode.Split('_');
                if (qr.Length == 2)
                {
                    var model = await Task.Run(() => db.app_XuatThanhPhamKhac_UpdateQRCode(NguoiDung, GhiChu,QRCode).FirstOrDefault());
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
