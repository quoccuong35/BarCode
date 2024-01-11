using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using DevExpress.Spreadsheet;
using Helpers;
using WMS.Models;
namespace WMS.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        Func fun = new Func();
        System.Web.Libs.Common com = new System.Web.Libs.Common();
        BaoCao cb = new BaoCao();
        /// <summary>
        /// 
        /// </summary>
        public void Excel()
        {
            string file = Server.MapPath("~/Content/upload/template/test.xlsx");

            DataTable dt = fun.GetDataFromExcelFile(file, 0, "A", "J",1);
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetBaoCao(string FileName, string DieuKienLoc1, string DieuKienLoc2)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new Workbook();
                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/test.xlsx"));
                Worksheet xlSheet = workbook.Worksheets[0];
                string con = com.GetConnectString(System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString);
                DataTable DT = com.DT_DataTable("SELECT   SoPO, MaSanPham, MaChiTietSanPham, SoLuongTrongMoiBo, SoLichGiaoHang, TenChiTietSanPham, TenQuanLyKhoCIRS, DonViTinh, SoLuongPO, SoLuongNhap,  NgayTao FROM dbo.BD_POChiTiet", con);
                if (DT.Rows.Count > 0)
                {
                    xlSheet.Import(DT, false, 4, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("TenBaoCaoLaDemo", FileName));
                st.code = 1;
            }
            catch (Exception ex)
            {
                st.code = 0;
                st.description = ex.Message;
            }
            return Json(st, JsonRequestBehavior.AllowGet);
        }
    }
}