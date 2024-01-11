using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Libs;
using System.Web.Mvc;
using WMS.Models;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using Helpers;

using WMS.DB;
using DevExpress.Spreadsheet;
using System.Drawing;

namespace WMS.Controllers
{
    [RoleAuthorize(Roles = "0,2=1")]
    public class PhieuNhapHangController : Controller
    {
        // GET: PhieuNhapHang
        WMSEntities db = new WMSEntities();
        GridData grid = new GridData();
        #region Danh sách phiếu nhập hàng

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetDanhSach(int Page, int PageSize, string Filter, bool isLoadingAll)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                st.data = grid.GetGridData("DanhSachPhieuNhapHang", Page, PageSize, Filter, isLoadingAll ? 1 : 0);
                st.code = 1;
            }
            catch (Exception ex)
            {
                st.code = 0;
                st.description = ex.Message;
            }
            var json = Json(st, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        #endregion
        public async Task<ActionResult> PhieuNhap(int? id)
        {

            var phieunhap = await db.BD_PhieuNhapHang.FirstOrDefaultAsync(m => m.IdPhieuNhapHang == id && m.Del != true);
            if (phieunhap == null)
            {
                phieunhap = new BD_PhieuNhapHang();
                phieunhap.IdPhieuNhapHang = 0;
                phieunhap.INTL = false;
                phieunhap.VN = true;
                phieunhap.Go = true;
                phieunhap.PhuKien = false;
                phieunhap.NgayGiaoHang = DateTime.Now.AddDays(1);
            }
            var PNHChiTiet = (from a in db.BD_PhieuNhapHangChiTiet
                              where a.IdPhieuNhapHang == id
                              group a by new { a.SoPO, a.SoLichGiaoHang, a.MaSanPham, a.MaChiTietSanPham, a.TenChiTietSanPham, a.DonViTinh } into g
                              select new PhieuNhapHangChiTiet
                              {
                                  SoPO = g.Key.SoPO,
                                  SoLichGiaoHang = g.Key.SoLichGiaoHang,
                                  MaSanPham = g.Key.MaSanPham,
                                  MaChiTietSanPham = g.Key.MaChiTietSanPham,
                                  TenChiTietSanPham = g.Key.TenChiTietSanPham,
                                  SoLuong = g.Sum(pc => pc.SoLuong),
                                  DonViTinh = g.Key.DonViTinh,
                              }).ToList();



            PhieuNhapHang model = new PhieuNhapHang();
            model.PNH = phieunhap;
            model.PNHCT = PNHChiTiet;

            return View(model);
        }
        public async Task<JsonResult> CheckPhieuNhap(int id, string SoPhieu)
        {

            return Json(await CheckPhieuNhapTonTai(id, SoPhieu), JsonRequestBehavior.AllowGet);
        }
        public async Task<bool> CheckPhieuNhapTonTai(int id, string SoPhieu)
        {
            bool is_exit = false;
            if ((id == 0 && await db.BD_PhieuNhapHang.CountAsync(m => m.SoPhieu == SoPhieu) == 0) || (id > 0 && await db.BD_PhieuNhapHang.CountAsync(m => m.SoPhieu == SoPhieu && m.IdPhieuNhapHang != id) == 0))
            {
                is_exit = false;
            }
            else
            {
                is_exit = true;
            }
            return is_exit;
        }
        [RoleAuthorize(Roles = "0,2=2")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ThemMoiPhieuNhapHang(BD_PhieuNhapHang phieunhap)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                if (await CheckPhieuNhapTonTai(0, phieunhap.SoPhieu))
                {
                    st.code = 0;
                    st.text = "Số phiếu nhập đã tồn tại";
                }
                else
                {
                    var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                    phieunhap.Del = false;
                    phieunhap.NgayTao = DateTime.Now;
                    phieunhap.IdNguoiTao = nguoidung.IdNguoiDung;
                    phieunhap.TrangThai = 1;
                    db.BD_PhieuNhapHang.Add(phieunhap);
                    await db.SaveChangesAsync();
                    st.code = (int)phieunhap.IdPhieuNhapHang;
                    st.text = "Thêm mới phiếu nhập thành công.";
                }

            }
            catch (Exception ex)
            {
                st.code = 0;
                st.description = ex.Message;
            }
            return Json(st);
        }
        [RoleAuthorize(Roles = "0,2=3")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CapNhatPhieuNhapHang(BD_PhieuNhapHang phieunhap)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                if (await CheckPhieuNhapTonTai((int)phieunhap.IdPhieuNhapHang, phieunhap.SoPhieu))
                {
                    st.code = 0;
                    st.text = "Số phiếu nhập đã tồn tại";
                }
                else if (phieunhap.TrangThai > 2)
                {
                    st.code = 0;
                    st.text = "Phiếu đã nhập hàng không thể chỉnh sửa.";
                }
                else
                {
                    var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                    var phieu = await db.BD_PhieuNhapHang.FirstOrDefaultAsync(m => m.IdPhieuNhapHang == phieunhap.IdPhieuNhapHang);
                    phieu.IdNhaCungCap = phieunhap.IdNhaCungCap;
                    phieu.NgaySua = DateTime.Now;
                    phieu.IdNguoiSua = nguoidung.IdNguoiDung;
                    phieu.Go = phieunhap.Go;
                    phieu.PhuKien = phieunhap.PhuKien;
                    phieu.NgayGiaoHang = phieunhap.NgayGiaoHang;
                    await db.SaveChangesAsync();
                    st.code = (int)phieunhap.IdPhieuNhapHang;
                    st.text = "Cập nhật phiếu nhập hàng thành công.";
                }

            }
            catch (Exception ex)
            {
                st.code = 0;
                st.description = ex.Message;
            }
            return Json(st);
        }
        [RoleAuthorize(Roles = "0,2=4")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DelPhieuNhapHang(int id)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var checkPhieuNhap = db.BD_PhieuNhapHangChiTiet.Where(it => it.IdPhieuNhapHang == id).ToList();
                if (checkPhieuNhap.Count > 0)
                {
                    st.code = 0;
                    st.text = "Phiếu nhập đã nhập hàng không thể xóa";
                    return Json(st,JsonRequestBehavior.AllowGet);
                }
                var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                var phieunhap = db.BD_PhieuNhapHang.FirstOrDefault(it => it.IdPhieuNhapHang == id);
                phieunhap.Del = true;
                phieunhap.NgaySua = DateTime.Now;
                phieunhap.IdNguoiSua = nguoidung.IdNguoiDung;
                db.SaveChanges();
                st.code = 1;
                st.text = "Thành công";
            }
            catch (Exception ex)
            {
                st.code = 0;
                st.description = ex.Message;
            }
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        [RoleAuthorize(Roles = "0,2=6")]
        public async Task<ActionResult> XuatFileCIRS()
        {
            return View();
        }

        [RoleAuthorize(Roles = "0,2=6")]
        public async Task<JsonResult> getXuatPhieuNhap(string SoPhieu,string FileName)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                var phieunhap = (from a in db.BD_PhieuNhapHang
                                 join b in db.HS_NhaCungCap on a.IdNhaCungCap equals b.IdNhaCungCap
                                 where a.SoPhieu.Trim() == SoPhieu.Trim()
                                 && a.Del != true
                                 select new { a.SoPhieu, b.MaNhaCungCap, b.TenNhaCungCap, a.NgayGiaoHang,a.IdPhieuNhapHang }).ToList();
                if (phieunhap.Count > 0)
                {
                    long id = phieunhap[0].IdPhieuNhapHang;
                    var PNHChiTiet = (from a in db.BD_PhieuNhapHangChiTiet
                                      where a.IdPhieuNhapHang == id
                                      group a by new { a.SoPO, a.SoLichGiaoHang, a.MaSanPham, a.MaChiTietSanPham, a.TenChiTietSanPham, a.DonViTinh } into g
                                      select new PhieuNhapHangChiTiet
                                      {
                                          SoPO = g.Key.SoPO,
                                          SoLichGiaoHang = g.Key.SoLichGiaoHang,
                                          MaSanPham = g.Key.MaSanPham,
                                          MaChiTietSanPham = g.Key.MaChiTietSanPham,
                                          TenChiTietSanPham = g.Key.TenChiTietSanPham,
                                          SoLuong = g.Sum(pc => pc.SoLuong),
                                          DonViTinh = g.Key.DonViTinh,
                                      }).ToList();
                    System.Web.Libs.Common com = new System.Web.Libs.Common();
                    BaoCao cb = new BaoCao();
                    cb.clearReport();
                    DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

                    workbook.LoadDocument(Server.MapPath("~/Content/upload/template/FileMauCIRS.xlsx"));
                    DevExpress.Spreadsheet.Worksheet xlSheet = workbook.Worksheets[0];
                    xlSheet.Cells["C5"].SetValue(SoPhieu);
                    xlSheet.Cells["C7"].SetValue(phieunhap[0].MaNhaCungCap);
                    xlSheet.Cells["E7"].SetValue(phieunhap[0].TenNhaCungCap);
                    xlSheet.Cells["C9"].SetValue(phieunhap[0].NgayGiaoHang.Value.ToString("dd/MM/yyyy"));
                    xlSheet.Cells["C11"].SetValue(phieunhap[0].NgayGiaoHang.Value.ToString("HH:mm"));

                    Style stylePO = workbook.Styles.Add("PO");
                    stylePO.Font.Size = 18;
                    stylePO.Font.Bold = true;
                    stylePO.Alignment.WrapText = true;
                    stylePO.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    stylePO.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                    Style styleMa = workbook.Styles.Add("Ma");
                    styleMa.Font.Size = 16;
                    styleMa.Font.Bold = true;
                    styleMa.Alignment.WrapText = true;
                    styleMa.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    styleMa.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                    Style styleSLNhap = workbook.Styles.Add("SLNhap");
                    styleSLNhap.Font.Size = 20;
                    styleSLNhap.Font.Bold = true;
                    styleSLNhap.Alignment.WrapText = true;
                    styleSLNhap.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    styleSLNhap.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                    int iStart = 18;
                    foreach (var item in PNHChiTiet)
                    {
                        xlSheet.Rows.Insert(iStart + 1);
                        xlSheet.Cells["A" + iStart.ToString()].Style = stylePO;
                        xlSheet.Cells["A" + iStart.ToString()].SetValue(item.SoPO.ToString());
                        xlSheet.Cells["B" + iStart.ToString()].Style = styleMa;
                        xlSheet.Cells["B" + iStart.ToString()].SetValue(item.SoLichGiaoHang.ToString());
                        xlSheet.Cells["C" + iStart.ToString()].Style = styleMa;
                        xlSheet.Cells["C" + iStart.ToString()].SetValue(item.MaSanPham.ToString());
                        xlSheet.Cells["D" + iStart.ToString()].Style = styleMa;
                        xlSheet.Cells["D" + iStart.ToString()].SetValue(item.MaChiTietSanPham.ToString());

                        xlSheet.MergeCells(xlSheet.Range["E" + iStart.ToString() + ":H" + (iStart).ToString()]);
                        xlSheet.Cells["E" + iStart.ToString()].Style = styleMa;
                        xlSheet.Cells["E" + iStart.ToString()].SetValue(item.TenChiTietSanPham.ToString());

                        xlSheet.MergeCells(xlSheet.Range["I" + iStart.ToString() + ":J" + (iStart).ToString()]);
                        xlSheet.Cells["I" + iStart.ToString()].Style = styleSLNhap;
                        xlSheet.Cells["I" + iStart.ToString()].SetValue(item.SoLuong.ToString());

                        xlSheet.MergeCells(xlSheet.Range["K" + iStart.ToString() + ":L" + (iStart).ToString()]);
                        xlSheet.Cells["K" + iStart.ToString()].Style = stylePO;
                        xlSheet.Cells["K" + iStart.ToString()].SetValue(item.DonViTinh.ToString());
                        xlSheet.MergeCells(xlSheet.Range["M" + iStart.ToString() + ":N" + (iStart).ToString()]);
                        iStart++;
                       
                    }
                    CellRange range1 = xlSheet.Range["A18:N" + (iStart - 1).ToString()];
                    range1.Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
                    workbook.SaveDocument(cb.fileName("FileMauCIRS", FileName));
                    rs.code = 1;
                }
                else
                {
                    rs.code = 0;
                    rs.description = "Không tìm thấy dữ liệu xuất";
                }
                                
                
            }
            catch (Exception ex)
            {
                rs.code = 0;
                rs.description = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [RoleAuthorize(Roles = "0,2=5")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DongPhieuNhap(int id)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                var phieunhap = db.BD_PhieuNhapHang.FirstOrDefault(it => it.IdPhieuNhapHang == id);
                phieunhap.NgaySua = DateTime.Now;
                phieunhap.IdNguoiSua = nguoidung.IdNguoiDung;
                phieunhap.TrangThai = 3;
                db.SaveChanges();
                st.code = 1;
                st.text = "Thành công";
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