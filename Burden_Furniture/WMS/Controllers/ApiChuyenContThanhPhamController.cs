using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/ChuyenCont")]
    public class ApiChuyenContThanhPhamController : ApiController
    {
        [HttpGet]
        [Route("DanhSach")]
        public async Task<List<app_DanhSachChuyenCont_Result>> DanhSach()
        {
            using (var db = new WMSEntities())
            {
                return await Task.Run(() => db.app_DanhSachChuyenCont().ToList());
            }
        }
        [HttpPost]
        [Route("QuetChuyenCont")]
        public async Task<JsonStatus> QuetChuyenCont(int NguoiDung, string MaQR1, string MaQR2)
        {
            using (WMSEntities db = new WMSEntities()) {
                JsonStatus st = new JsonStatus();
                try
                {
                    long IDLenhSX1 = 0, IDLenhSX2 = 0;int SoOf1 = 0, SoOf2 = 0;
                    string sLoi = "";

                    try
                    {
                        var arr = MaQR1.Split('_');
                        if (arr.Length == 2)
                        {
                            IDLenhSX1 = long.Parse(arr[0]);
                            SoOf1 = int.Parse(arr[1]);
                        }
                        else
                        {
                            st.code = 0;
                            sLoi += "QRCode1 không hợp lệ.";
                        }
                    }
                    catch 
                    {
                        st.code = 0;
                        sLoi += "QRCode1 không hợp lệ.";
                    }
                    try
                    {
                        var arr1 = MaQR2.Split('_');
                        if (arr1.Length == 2)
                        {
                            IDLenhSX2 = long.Parse(arr1[0]);
                            SoOf2 = int.Parse(arr1[1]);
                        }
                        else
                        {
                            st.code = 0;
                            sLoi += "QRCode2 không hợp lệ.";
                        }
                    }
                    catch 
                    {

                        st.code = 0;
                        sLoi += "QRCode2 không hợp lệ.";
                    }

                    if (sLoi.Length == 0)
                    {
                        var model = await Task.Run(() => db.app_ChuyenCont(MaQR1, MaQR2, IDLenhSX1, IDLenhSX2, SoOf1, SoOf2,NguoiDung).FirstOrDefault());
                        st.code = (int)model.code;
                        st.text = model.text;
                    }
                    else
                    {
                        st.code = 0;
                        st.text = sLoi; ;
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
}
