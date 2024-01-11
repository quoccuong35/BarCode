using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using WMS.DB;
using WMS.Models;

namespace WMS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        WMSEntities db = new WMSEntities();
        public ActionResult Index(string id)
        {
            Common con = new Common();
            var reportModel = new ReportsModel();
            reportModel.ReportName = "Test";
            reportModel.Report = XtraReport.FromFile(Server.MapPath("~/Content/upload/reports/InQRCode1.repx"), true);
            string sql = " SELECT SoPO, SoLichGiaoHang, MaSanPham, MaChiTietSanPham, TenQuanLyKhoCIRS, DonViTinh, " +
                          "  SoPO+'@' + SoLichGiaoHang + '@' + MaSanPham + '@' + MaChiTietSanPham + '@' + TenQuanLyKhoCIRS + '@' + DonViTinh AS QRCode " +
                           "  FROM dbo.BD_POChiTiet WHERE IDPO = 4";
            reportModel.Report.DataSource = con.DT_DataTable(sql, con.GetConnectString(System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString));
            if (reportModel.Report.Parameters["par_TuNgay"] != null)
            {
                reportModel.Report.Parameters["par_TuNgay"].Value = DateTime.Now.ToString();
            }
            return View(reportModel);
        }

        public ActionResult InQRCodeKe(string id)
        {
            string[] sarrayKe = id.Split(';');
            List<int> listKe = new List<int>();
            for (int i = 0; i < sarrayKe.Length; i++)
            {
                if (sarrayKe[i].Trim() == null || sarrayKe[i].Trim() == "")
                    break;
                listKe.Add(int.Parse(sarrayKe[i].Trim()));
            }
            string KyHieu = db.Sys_CauHinh.FirstOrDefault(it => it.TuKhoa == "QRCode_KyHieuKe").GiaTri;
            var model = db.HS_Ke.Where(it => listKe.Contains(it.IdKe)).Select(it => new { it.MaKe, QRCodeKe = KyHieu + it.IdKe.ToString() }).ToList();
            var reportModel = new ReportsModel();
            reportModel.ReportName = "InQRCodeKe";
            reportModel.Report = XtraReport.FromFile(Server.MapPath("~/Content/upload/reports/MauQRCodeKe.repx"), true);
            reportModel.Report.DataSource = model;
            return View(reportModel);
        }
    }
}