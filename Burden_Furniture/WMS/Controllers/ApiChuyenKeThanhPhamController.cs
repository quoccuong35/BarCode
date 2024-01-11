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
    [RoutePrefix("api/ChuyenKeThanhPham")]
    public class ApiChuyenKeThanhPhamController : ApiController
    {
        WMSEntities db = new WMSEntities();
        [HttpGet]
        [Route("ChiTiet")]
        public async Task<List<App_ChuyenKeThanhPham_ChiTiet_Result>> ChiTiet(int Id)
        {
            return await Task.Run(() => db.App_ChuyenKeThanhPham_ChiTiet(Id).ToList());
        }
        [HttpPost]
        [Route("ChuyenKe")]
        public async Task<JsonStatus> ChuyenKe(int NguoiDung, int IdKe, string QRCode)
        {
            JsonStatus js = new JsonStatus();
            var kq = db.App_ChuyenKeThanhPham_UpdateKe(NguoiDung, IdKe, QRCode).ToList();
            js.code = kq[0].code.Value;
            js.text = kq[0].text;
            if (js.code == 1)
            {
                js.data = Task.Run(() => db.App_ChuyenKeThanhPham_ChiTietKe(NguoiDung, IdKe).ToList());
            }
            return js;
        }
        [HttpGet]
        [Route("ChiTietKe")]
        public async Task<List<App_ChuyenKeThanhPham_ChiTietKe_Result>> ChiTietKe(int NguoiDung, int IdKe)
        {
            return await Task.Run(() => db.App_ChuyenKeThanhPham_ChiTietKe(NguoiDung, IdKe).ToList());
        }
    }
}
