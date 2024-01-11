using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Helpers;
using System.Threading.Tasks;
using WMS.DB;
using WMS.Models;
using DevExpress.Spreadsheet;
using System.Web.Libs;
using System.Drawing;
using System.Transactions;

namespace WMS.Controllers
{
    public class BaoCaoController : Controller
    {
        // GET: BaoCao
        BaoCao cb = new BaoCao();
        WMSEntities db = new WMSEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SpreadsheetPartial()
        {
            return PartialView("_SpreadsheetPartial", cb.reportFileFullName());
        }
        public ActionResult DownloadXls()
        {
            return File(cb.fileReport(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", cb.reportName());
        }
        [RoleAuthorize(Roles = "0,7=1")]
        public async Task<ActionResult> BaoCaoTienDoTrongSanXuat()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,7=1")]
        public async Task<JsonResult> getBaoCaoTienDonTrongSanXuat(DateTime TuNgay, DateTime DenNgay,string FileName,string SoEPI)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                var model = db.BD_BaoCaoTienDoSanXuat(TuNgay, DenNgay,SoEPI).ToList();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/FileMauBaoCaoTienDoSanXuat.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets["General"];
                xlSheet.Import(model, 4, 0);// List hoặc DataTable
                workbook.Worksheets["General"].ActiveView.Zoom = 70;
                workbook.SaveDocument(cb.fileName("FileMauBaoCaoTienDoSanXuat", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.code = 0;
                rs.description = ex.Message;
            }
            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,15=1")]
        public async Task<ActionResult> BaoCaoTienDoTrongSanXuatChiTiet()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,15=1")]
        public async Task<JsonResult> getBaoCaoTienDoTrongSanXuatChiTiet(DateTime TuNgay, DateTime DenNgay, string FileName,string maCongDoan,string soEPI)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                string tenCongDoan = "";
                if (maCongDoan == "CD1")
                {
                    tenCongDoan = "Qty.1 Hàng trắng";
                }
                else if (maCongDoan == "CD2")
                {
                    tenCongDoan = "Qty.2 Mộc";
                }
                else if (maCongDoan == "CD3")
                {
                    tenCongDoan = "Qty.3 Sơn";
                }
                else if (maCongDoan == "CD4")
                {
                    tenCongDoan = "Qty.4 Xuất Khẩu";
                }
                else if (maCongDoan == "CD5")
                {
                    tenCongDoan = "Qty.5 QC7";
                }
                else if (maCongDoan == "CD6")
                {
                    tenCongDoan = "Qty.6 Đóng gói";
                }
                else if (maCongDoan == "CD7")
                {
                    tenCongDoan = "Xuất hàng";
                }
                var model = db.BD_BaoCaoTienDoSanXuatChiTiet(TuNgay, DenNgay, maCongDoan,soEPI).ToList();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/FileMauBaoCaoTienDoSanXuatChiTiet.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets["Data"];
                xlSheet.Cells["O3"].SetValue(tenCongDoan);
                xlSheet.Import(model, 4, 0);// List hoặc DataTable
                                           
                workbook.SaveDocument(cb.fileName("FileMauBaoCaoTienDoSanXuatChiTiet", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.code = 0;
                rs.description = ex.Message;
            }

            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,6=1")]
        public async Task<ActionResult> BaoCaoXuatKeHangTrang() {
            return View();
        }
        [RoleAuthorize(Roles = "0,6=1")]
        public async Task<JsonResult> getBaoCaoXuatKeHangTrang(DateTime TuNgay, DateTime DenNgay, string FileName,String MaKe,string SoEPI, string MaSKU,string MaSP)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                var model = db.BC_TheoDoiXuatKeHangTrang(TuNgay, DenNgay, MaKe,SoEPI, MaSKU,MaSP).ToList();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/FileMauXuatKeHangTrang.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 3, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("FileMauXuatKeHangTrang", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.code = 0;
                rs.description = ex.Message;
            }

            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [RoleAuthorize(Roles = "0,5=1")]
        public async Task<ActionResult> BaoCaoTheoDoiNhapHangTrang()
        {
            return View();
        }

        [RoleAuthorize(Roles = "0,5=1")]
        public async Task<JsonResult> getBaoCaoTheoDoiNhapHangTrang(DateTime TuNgay, DateTime DenNgay,String FileName,string MaKe,string MaQuanLyKho,string MaSP)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                var model = db.BD_BaoCaoNhapHangTrang(TuNgay, DenNgay,MaQuanLyKho,MaKe,MaSP).ToList();
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
              
                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/BaoCaoNhapHangTrang.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 3, 1);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("BaoCaoNhapHangTrang", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {

                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> XuatFileCLS()
        {
            return View();
        }
        public async Task<JsonResult> getFileCLS(string SoEPI, String FileName)
        {
            JsonStatus st = new JsonStatus();
            st.code = 0;
            st.text = "Lỗi";
            string sLoi = "";
            var data = db.BD_LayThongTinCLS(SoEPI.Trim()).ToList();
            if (data.Count == 0)
            {
               st.code = 0;
               st.text = "Không tìm thấy thông tin EPI";
               return Json(st, JsonRequestBehavior.AllowGet);
            }
            try
            {
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/FileMauCLS.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                int iStart = 18;string MaSanPham = "", MaKhachHang= "", MaThung = "";
                xlSheet.Cells["K4"].SetValue(DateTime.Now.ToString("dd/MM/yyyy"));
                xlSheet.Cells["K7" ].SetValue(SoEPI);
                Style styleMaSanPham = workbook.Styles.Add("MaSanPham");
                styleMaSanPham.Font.Size = 9;
                //styleMaSanPham.Font.Bold = true;
                styleMaSanPham.Alignment.WrapText = true;
                styleMaSanPham.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                styleMaSanPham.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                //Style styleMaChiTiet = workbook.Styles.Add("MaChiTiet");
                //styleMaChiTiet.Font.Size = 26;
                //styleMaChiTiet.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                //styleMaChiTiet.Alignment.WrapText = true;
                //styleMaChiTiet.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                //Style styleMaKhachHang = workbook.Styles.Add("MaKhachHang");
                //styleMaKhachHang.Font.Size = 22;
                //styleMaKhachHang.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                //styleMaKhachHang.Alignment.WrapText = true;
                //styleMaKhachHang.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                //Style styleSoLuong = workbook.Styles.Add("SoLuong");
                //styleSoLuong.Font.Size = 48;
                //styleSoLuong.Font.Bold = true;
                //styleSoLuong.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                //styleSoLuong.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                //Style styleMaThung = workbook.Styles.Add("MaThung");
                //styleMaThung.Font.Size = 24;
                //styleMaThung.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                //styleMaThung.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                foreach (var item in data)
                {
                    if (MaSanPham.Trim() == item.MaSanPham.Trim())
                    {
                        //xlSheet.Cells["A" + iStart.ToString() + ":A" + (iStart - 1).ToString()].Merge();
                        xlSheet.MergeCells(xlSheet.Range["A" + iStart.ToString() + ":A" + (iStart - 1).ToString()]);
                    }
                    else
                    {
                        xlSheet.Cells["A" + iStart.ToString()].Style = styleMaSanPham;
                        xlSheet.Cells["A" + iStart.ToString()].SetValue(item.MaSanPham.ToString());
                    }
                    xlSheet.Cells["B" + iStart.ToString()].Style = styleMaSanPham;
                    xlSheet.Cells["B" + iStart.ToString()].SetValue(item.MaChiTietSanPham.ToString());
                    if (MaKhachHang.Trim() == item.MaHangKhH.Trim() && MaSanPham.Trim() == item.MaSanPham.Trim())
                    {
                        // xlSheet.Cells["C" + iStart.ToString() + ":C" + (iStart - 1).ToString()].Merge();
                        xlSheet.MergeCells(xlSheet.Range["C" + iStart.ToString() + ":C" + (iStart - 1).ToString()]);
                    }
                    else
                    {
                        xlSheet.Cells["C" + iStart.ToString()].Style = styleMaSanPham;
                        xlSheet.Cells["C" + iStart.ToString()].SetValue(item.MaHangKhH.ToString());
                    }
                    xlSheet.Cells["D" + iStart.ToString()].Style = styleMaSanPham;
                    xlSheet.Cells["D" + iStart.ToString()].SetValue(item.MoTa.ToString());
                    xlSheet.Cells["E" + iStart.ToString()].Style = styleMaSanPham;
                    xlSheet.Cells["E" + iStart.ToString()].SetValue(item.MaMauOrVai.ToString());
                    xlSheet.Cells["G" + iStart.ToString()].Style = styleMaSanPham;
                    xlSheet.Cells["G" + iStart.ToString()].SetValue(item.SoEPI);
                    if (MaThung.Trim() == item.MaThung.Trim() && MaSanPham.Trim() == item.MaSanPham.Trim())
                    {
                        xlSheet.MergeCells(xlSheet.Range["H" + iStart.ToString() + ":H" + (iStart - 1).ToString()]);
                    }
                    else
                    {
                        xlSheet.Cells["H" + iStart.ToString()].Style = styleMaSanPham;
                        xlSheet.Cells["H" + iStart.ToString()].SetValue(item.MaThung.ToString());
                    }
                   // xlSheet.MergeCells(xlSheet.Range["H" + iStart.ToString() + ":K" + (iStart).ToString()]);
                   // xlSheet.Cells["H" + iStart.ToString()].Style = styleMaSanPham;
                   // xlSheet.Cells["L" + iStart.ToString()].Style = styleMaSanPham;
                    xlSheet.Cells["J" + iStart.ToString()].SetValue(item.SoLuong.ToString());
                    MaThung = item.MaThung;MaKhachHang = item.MaHangKhH;MaSanPham = item.MaSanPham;
                    xlSheet.Rows.Insert(iStart + 1);
                    iStart++;
                }
                xlSheet.Cells["D"+(iStart+2)].SetValue(SoEPI);
                CellRange range1 = xlSheet.Range["A18:K"+(iStart-1).ToString()];
                range1.Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
                workbook.SaveDocument(cb.fileName("FileMauCLS",FileName));
                st.code = 1;

            }
            catch (Exception ex)
            {
                st.text = ex.Message;
                st.code = 0;
            }


            return Json(st, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,13=1")]
        public async Task<ActionResult> BaoCaoTheoDoiKeThanhPham()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,13=1")]
        public async Task<JsonResult> getBaoCaoTheoDoiKeThanhPham(string FileName,string MaKe,string MaSKU,string MaSP,string SoEPI) {
            DateTime DenNgay = DateTime.Now.Date,TuNgay = new DateTime(DenNgay.Year,DenNgay.Month,1);
            
            JsonStatus rs = new JsonStatus();
            try
            {
                var model = db.BD_BaoCaoTheoDoiKeThanhPham(TuNgay, DenNgay,MaSKU,MaKe,MaSP,SoEPI).ToList();
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/BaoCaoTheoDoiKeThanhPham.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 4, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("BaoCaoTheoDoiKeThanhPham", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {

                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,14=1")]
        public async Task<ActionResult> BaoCaoTheoDoiKeHangTrang()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,14=1")]
        public async Task<JsonResult> getBaoCaoTheoDoiKeHangTrang(string FileName,string MaSKU,string MaSP,string MaKe)
        {
            DateTime DenNgay = DateTime.Now.Date, TuNgay = new DateTime(DenNgay.Year, DenNgay.Month, 1);

            JsonStatus rs = new JsonStatus();
            try
            {
                var model = db.BD_BaoCaoTongHopKeHangTrang(TuNgay, DenNgay,MaSKU,MaSP,MaKe).ToList();
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/BaoCaoTheoDoiKeHangTrang.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 4, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("BaoCaoTheoDoiKeHangTrang", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {

                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,16=1")]
        public async Task<ActionResult> BaoCaoTheoDoiNhapKeThanhPham()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,16=1")]
        public async Task<JsonResult> getBaoCaoTheoDoiNhapKeThanhPham(DateTime TuNgay, DateTime DenNgay, String FileName, string MaKe, string MaSKU,string MaSP,string SoEPI)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                var model = db.BD_BaoCaoTheoDoiNhapKeThanhPham(TuNgay, DenNgay, MaSKU, MaKe,MaSP,SoEPI).ToList();
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/BaoCaoNhapThanhPham.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 3, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("BaoCaoNhapThanhPham", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {

                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,17=1")]
        public async Task<ActionResult> BaoCaoTheoDoiXuatKeThanhPham()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,17=1")]
        public async Task<JsonResult> getBaoCaoTheoDoiXuatKeThanhPham(DateTime TuNgay, DateTime DenNgay, String FileName, string MaKe, string MaSKU,string MaSP, string SoEPI)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                var model = db.BD_TheoDoiXuatKeThanhPham(TuNgay, DenNgay, MaSKU, MaKe,SoEPI,MaSP).ToList();
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/BaoCaoXuatThanhPham.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 3, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("BaoCaoXuatThanhPham", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {

                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> BaoCaoTongTonTheoSKU()
        {
            return View();
        }
        public async Task<JsonResult> getBaoCaoTongTonTheoSKU(string FileName, string MaSP,string MaSKU)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                var model = db.BD_BaoCaoTongTon(MaSP, MaSKU).ToList();
                System.Web.Libs.Common com = new System.Web.Libs.Common();
                cb.clearReport();
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

                workbook.LoadDocument(Server.MapPath("~/Content/upload/template/MauBaoCaoTongTon.xlsx"));
                DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                if (model.Count > 0)
                {
                    //var tem = model;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    model.AddRange(tem);
                    //}
                    xlSheet.Import(model, 3, 0);// List hoặc DataTable
                }
                workbook.SaveDocument(cb.fileName("BaoCaoXuatThanhPham", FileName));
                rs.code = 1;
            }
            catch (Exception ex)
            {

                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,21=1")]
        public async Task<ActionResult> BaoCaoChiTietHangThanhPhamTrongKho()
        {
            return View();
        }
        public async Task<JsonResult> getBaoCaoChiTietHangThanhPhamTrongKho( string MaKe, string MaSKU, string MaSP, string SoEPI)
        {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            try
            {
                rs.data = db.BD_BaoCaoChiTietHangThanhPhamTrongKho(MaSKU, MaKe, MaSP, SoEPI).ToList();
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.text = ex.Message;
            }
          
            var rss=  Json(rs, JsonRequestBehavior.AllowGet);
            rss.MaxJsonLength = int.MaxValue;
            return rss;
        }
        [HttpPost]
        public JsonResult XoaKeThanhPham(long id) {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            try
            {
                using (var tran = new TransactionScope())
                {
                    var del = db.BD_NhapKhoThanhPham.FirstOrDefault(it => it.Id == id);
                    if (del != null)
                    {
                        var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                        HT_HistoryEditData edit = new HT_HistoryEditData();
                        edit.TableName = "BD_NhapKhoThanhPham";
                        edit.KeyRow = id.ToString();
                        edit.NgaySua = DateTime.Now;
                        edit.NguoiDungSua = nguoidung.IdNguoiDung;
                        db.BD_NhapKhoThanhPham.Remove(del);
                        db.HT_HistoryEditData.Add(edit);
                        if (db.SaveChanges() > 0)
                        {
                            tran.Complete();
                            rs.code = 1;
                            rs.text = "Thành công";
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                rs.text = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> BaoCaoChiTietKhoHangTrang()
        {
            return View();
        }
        public async Task<ActionResult> GetBaoCaoChiTietKhoHangTrang(string MaKe, string MaSKU, string MaSP)
        {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            try
            {
                rs.data = db.BD_BaoCaoNhapHangTrangChiTiet(MaSP, MaSKU, MaKe).ToList();
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.text = ex.Message;
            }

            var rss = Json(rs, JsonRequestBehavior.AllowGet);
            rss.MaxJsonLength = int.MaxValue;
            return rss;
        }

        public async Task<ViewResult> BaoCaoTrangThaiSanXuatQRCode() {
            return View();
        }
        public async Task<JsonResult> GetBaoCaoTrangThaiSanXuatQRCode(String soEPI, String maSKU)
        {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            try
            {
                rs.data = db.Proc_XemTienDoSanXuat(soEPI, maSKU).ToList();
                rs.code = 1;
            }
            catch (Exception ex)
            {
                rs.text = ex.Message;
            }

            var rss = Json(rs, JsonRequestBehavior.AllowGet);
            rss.MaxJsonLength = int.MaxValue;
            return rss;
        }
    }
}