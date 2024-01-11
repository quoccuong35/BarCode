using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;
using System.Transactions;
using System.Web.Libs;
using WMS.DB;
using DevExpress.XtraReports.UI;
using System.ComponentModel;


namespace WMS.Controllers
{
    [RoleAuthorize(Roles = "0,3=1,19=1")]
    [Authorize]
    public class LenhRaiHangController : Controller
    {
        WMSEntities db = new WMSEntities();
        GridData grid = new GridData();
        // GET: LenhRaiHang
        [RoleAuthorize(Roles = "0,3=1")]
        public ActionResult Index()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,3=1")]
        public JsonResult GetDanhSach(int Page, int PageSize, string Filter, bool isLoadingAll)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                st.data = grid.GetGridData("DanhSachLenhRaiHang", Page, PageSize, Filter, isLoadingAll ? 1 : 0);
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
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,3=2")]
        public async Task<JsonResult> ImportFileExcel(HttpPostedFileBase importFile)
        {
            JsonStatus st = new JsonStatus();
            st.code = 0;
            st.text = "Lỗi";
            string sLoi = "";
            try
            {
                if (importFile != null && importFile.ContentLength > 0 && (Path.GetExtension(importFile.FileName).Equals(".xlsx")))
                {
                    string fileName = importFile.FileName;
                    fileName = User.Identity.Name + "_" + fileName;
                    string UploadDirectory = Server.MapPath("~/Content/upload/");
                    bool folderExists = System.IO.Directory.Exists(UploadDirectory);
                    if (!folderExists)
                        System.IO.Directory.CreateDirectory(UploadDirectory);
                    string resultFilePath = UploadDirectory + fileName;
                    importFile.SaveAs(resultFilePath);
                    //DataTable dt = await getDataTableFromExcel(resultFilePath);

                    List<BD_LenhRaiHang> model = getDataTableFromExcel(resultFilePath, ref sLoi);
                    if (sLoi.Length > 0)
                    {
                        st.code = 0;
                        st.text = sLoi;
                    }
                    else if(model.Count> 0)
                    {
                        List<string> EPI = model.Select(it => it.SoEPI).Distinct().ToList();
                        List<String> maChiTiet = model.Select(it => it.MaChiTietSanPham).Distinct().ToList();
                        var listKiemTra = db.BD_LenhRaiHang.Where(it => EPI.Contains(it.SoEPI) && maChiTiet.Contains(it.MaChiTietSanPham.Trim())).ToList();
                        if (listKiemTra.Count > 0)
                        {
                            //List<String> soCont = listKiemTra.Select(it => it.SoEPI).Distinct().ToList();
                            //List<String> maChiTiet = listKiemTra.Select(it => it.MaChiTietSanPham).Distinct().ToList();
                            //List<String> soPo = listKiemTra.Select(it => it.SoPO).Distinct().ToList();
                            //var checkData = model.Where(it => soCont.Contains(it.SoEPI.Trim()) && maChiTiet.Contains(it.MaChiTietSanPham.Trim())
                            //                          && soPo.Contains(it.SoPO.Trim())).ToList();
                            foreach (var add in listKiemTra)
                            {
                                var item = model.Where(it => it.SoEPI == add.SoEPI && it.MaChiTietSanPham.Trim() == add.MaChiTietSanPham && it.SoPO.Trim() == add.SoPO && it.MaMauOrVai == add.MaMauOrVai && it.ODM == add.ODM && it.MaThung == add.MaThung && it.MaSanPham == add.MaSanPham && it.MaKH == add.MaKH).FirstOrDefault();
                                if (item != null) {
                                    sLoi += "Đã tồn tại số EPI <b>" + item.SoEPI + "</b> Của mã SKU <b>" + item.MaChiTietSanPham + "</b> Của PO <b>" + item.SoPO + "</b> trong hệ thống </br>";
                                }
                            }
                            if (sLoi.Length > 0)
                            {
                                st.code = 0;
                                st.text = sLoi;
                                return Json(st);
                            }
                            //if (checkData.Count > 0)
                            //{
                            //    foreach (var item in checkData)
                            //    {
                            //        sLoi += "Đã tồn tại số EPI <b>" + item.SoEPI + "</b> Của mã SKU <b>" + item.MaChiTietSanPham + "</b> Của PO <b>" + item.SoPO + "</b> trong hệ thống </br>";
                            //    }
                            //    st.code = 0;
                            //    st.text = sLoi;
                            //    return Json(st);
                            //}
                        }

                        using (var tran = new TransactionScope())
                        {
                            db.BD_LenhRaiHang.AddRange(model);
                            int stt = db.SaveChanges();
                            if (stt>0)
                            {
                                st.code = 1;
                                st.text = "Thành công " + stt.ToString();
                                tran.Complete();
                            }
                        }
                    }
                }
                return Json(st);
            }
            catch (Exception)
            {
                return Json(st);
            }
        }
        public List<BD_LenhRaiHang> getDataTableFromExcel(string path, ref string sLoi)
        {
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            List<BD_LenhRaiHang> data = new List<BD_LenhRaiHang>();
            using (XLWorkbook workBook = new XLWorkbook(path))
            {
                IXLWorksheet workSheet = workBook.Worksheet(1);
                bool firstRow = true;
                int i = 0;
                string SoEPI = "";
                foreach (IXLRow row in workSheet.Rows())
                {
                    SoEPI = row.Cell(1).Value.ToString();
                    if (SoEPI == "")
                        break;
                    if (firstRow)
                    {
                        //foreach (IXLCell cell in row.Cells())
                        //{
                        //    dt.Columns.Add(cell.Value.ToString());
                        //}
                        firstRow = false;
                        continue;
                    }
                    else
                    {
                        
                        BD_LenhRaiHang add = new BD_LenhRaiHang();
                        add.SoEPI = SoEPI;
                        add.STT = i;
                        add.SoPO = row.Cell(2).Value.ToString().Trim();
                        add.MaSanPham = row.Cell(3).Value.ToString().Trim();
                        add.MaChiTietSanPham = row.Cell(4).Value.ToString().Trim();
                        add.MaKH = row.Cell(5).Value.ToString().Trim();
                        add.MoTa = row.Cell(6).Value.ToString().Trim();
                        add.KichThuocSanPham = row.Cell(7).Value.ToString().Trim();
                        add.MaMauOrVai = row.Cell(8).Value.ToString().Trim();
                        add.ODM = row.Cell(9).Value.ToString().Trim();
                        add.TayNam = row.Cell(10).Value.ToString().Trim();
                        add.MaThung = row.Cell(11).Value.ToString().Trim();
                        add.MauTrong = row.Cell(12).Value.ToString().Trim();
                        add.Del = false;
                        try
                        {
                            add.SoLuongEPI = int.Parse(row.Cell(13).Value.ToString().Trim());
                        }
                        catch
                        {
                            sLoi += " Lỗi số lượng EPI không hợp lệ dòng" + i.ToString();
                            continue;
                        }
                        add.LoaiDongGoi = row.Cell(14).Value.ToString().Trim();
                        //try
                        //{
                        //    add.STT = int.Parse(row.Cell(11).Value.ToString().Trim());
                        //}
                        //catch
                        //{
                        //    sLoi += " Số STT chạy mã không hợp lệ dòng " + i.ToString();
                        //    continue;
                        //}
                        add.MaPhoiHop = add.SoEPI +  add.MaChiTietSanPham + add.MaMauOrVai +add.ODM +  add.STT.ToString();
                        add.NguoiDungTao = nguoidung.IdNguoiDung;
                        add.NgayTao = DateTime.Now;
                        if (data.Count > 0)
                        {
                            var CheckEPI = data.Where(it => it.SoEPI == add.SoEPI && it.MaChiTietSanPham.Trim() == add.MaChiTietSanPham && it.SoPO.Trim() == add.SoPO && it.MaMauOrVai == add.MaMauOrVai && it.ODM == add.ODM && it.MaThung == add.MaThung && it.MaSanPham == add.MaSanPham && it.MaKH == add.MaKH).ToList();
                            if (CheckEPI.Count > 0)
                            {
                                sLoi += "Thông tin số " + add.SoEPI + " " + add.MaChiTietSanPham + " của " + add.SoPO + i.ToString() + " đã tồn tại không thể thêm";
                                continue;
                            }
                        }
                        data.Add(add);
                        i++;
                    }
                }
                return data;
            }
        }

      //  [RoleAuthorize(Roles = "0")]
        public async Task<JsonResult> Edit(BD_LenhRaiHang item, BD_LenhRaiHang old)
        {
            JsonStatus st = new JsonStatus();
            st.code = 0;
            st.text = "Thất bại";
            if (!User.IsInRole("0") && User.Identity.Name != "pm.inven")
            {
                st.code = 0;
                st.text = "Bạn chưa được phân quyền sửa";
                return Json(st, JsonRequestBehavior.AllowGet);
            }
           
            //var checkXuatHang = db.BD_PhieuNhapHangChiTiet.Where(it => it.SoEPI == old.SoEPI && it.MaChiTietSanPham == old.MaChiTietSanPham).ToList();
            //if (checkXuatHang.Count > 0)
            //{
            //    st.text = "Mã <b>SKU " + old.MaChiTietSanPham + "</b> đã xuất hàng không thể sửa";
            //}
            //else
            //{
                
            //}
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            var model = db.BD_LenhRaiHang.FirstOrDefault(it => it.IDLenhRaiHang == item.IDLenhRaiHang);
            DateTime dt = DateTime.Now;
            model.NguoiDungSua = nguoidung.IdNguoiDung;
            model.NgaySua = dt;
            List<HT_HistoryEditData> addH = new List<HT_HistoryEditData>();
            if (item.SoEPI != null)
            {
                
                addH.Add(new HT_HistoryEditData { ColumnName = "SoEPI", OldValue = model.SoEPI, NewValue = item.SoEPI, TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.SoEPI = item.SoEPI;
            }
            if (item.MaSanPham != null)
            {
                
                addH.Add(new HT_HistoryEditData { ColumnName = "MaSanPham", OldValue = model.MaSanPham, NewValue = item.MaSanPham, TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MaSanPham = item.MaSanPham;
            }
            if (item.MaChiTietSanPham != null)
            {
                
                addH.Add(new HT_HistoryEditData { ColumnName = "MaChiTietSanPham", OldValue = model.MaChiTietSanPham, NewValue = item.MaChiTietSanPham, TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MaChiTietSanPham = item.MaChiTietSanPham;
            }
            if (item.SoLuongEPI != null)
            {
               
                addH.Add(new HT_HistoryEditData { ColumnName = "SoLuongEPI", OldValue = model.SoLuongEPI.ToString(), NewValue = item.SoLuongEPI.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.SoLuongEPI = item.SoLuongEPI;
            }
            if (item.SoPO != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "SoPO", OldValue = model.SoPO.ToString(), NewValue = item.SoPO.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.SoPO = item.SoPO;
            }
            if (item.MaKH != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "MaKH", OldValue = model.MaKH.ToString(), NewValue = item.MaKH.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MaKH = item.MaKH;
            }
            if (item.ODM != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "ODM", OldValue = model.ODM.ToString(), NewValue = item.ODM.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.ODM = item.ODM;
            }
            if (item.MoTa != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "MoTa", OldValue = model.MoTa.ToString(), NewValue = item.MoTa.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MoTa = item.MoTa;
            }
            if (item.KichThuocSanPham != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "KichThuocSanPham", OldValue = model.KichThuocSanPham.ToString(), NewValue = item.KichThuocSanPham.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.KichThuocSanPham = item.KichThuocSanPham;
            }
            if (item.MaMauOrVai != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "MaMauOrVai", OldValue = model.MaMauOrVai.ToString(), NewValue = item.MaMauOrVai.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MaMauOrVai = item.MaMauOrVai;
            }
            if (item.TayNam != null)
            {
                addH.Add(new HT_HistoryEditData { ColumnName = "TayNam", OldValue = model.TayNam.ToString(), NewValue = item.TayNam.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.TayNam = item.TayNam;
            }
            if (item.MaThung != null)
            {
                
                addH.Add(new HT_HistoryEditData { ColumnName = "MaThung", OldValue = model.MaThung.ToString(), NewValue = item.MaThung.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MaThung = item.MaThung;
            }
            if (item.LoaiDongGoi != null)
            {
              
                addH.Add(new HT_HistoryEditData { ColumnName = "LoaiDongGoi", OldValue = model.LoaiDongGoi.ToString(), NewValue = item.LoaiDongGoi.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.LoaiDongGoi = item.LoaiDongGoi;
            }
            if (item.MauTrong != null)
            {
                
                addH.Add(new HT_HistoryEditData { ColumnName = "MauTrong", OldValue = model.MauTrong.ToString(), NewValue = item.MauTrong.ToString(), TableName = "BD_LenhRaiHang", KeyRow = model.IDLenhRaiHang.ToString(), NguoiDungSua = nguoidung.IdNguoiDung, NgaySua = dt });
                model.MauTrong = item.MauTrong;
            }
            if (addH.Count > 0)
                db.HT_HistoryEditData.AddRange(addH);
            db.SaveChanges();
            st.code = 1;
            st.text = "Thành công";
            var json = Json(st, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;

        }
        [RoleAuthorize(Roles = "0,3=4")]
        public async Task<JsonResult> Del(BD_LenhRaiHang item)
        {
            JsonStatus st = new JsonStatus();
            st.code = 0;
            st.text = "Thất bại";

            var check = db.BD_QuetSanXuat.Where(it => it.IDLenhRaiHang == item.IDLenhRaiHang).ToList();
            if (check.Count > 0)
            {
                st.text = "Đã quét sản xuất không thể xóa";
                st.code = 0;
            }
            else
            {
                var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                var model = db.BD_LenhRaiHang.FirstOrDefault(it => it.IDLenhRaiHang == item.IDLenhRaiHang);
                model.Del = true;
                model.NguoiDungSua = nguoidung.IdNguoiDung;
                model.NgaySua = DateTime.Now;
                db.SaveChanges();
                st.code = 1;
                st.text = "Thành công";
            }

            var json = Json(st, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;

        }
        [HttpGet]
        [RoleAuthorize(Roles = "0,3=6")]
        public async Task<ActionResult> InMauQRCode2()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,3=6")]
        public async Task<JsonResult> getDataInQRCoce(string SoEPI)
        {
            var model = db.BD_InQRCode2(SoEPI).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,3=6")]
        public async Task<JsonResult> addSession(List<BD_InQRCode2_Result> list)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            Session["QRCode02"] = null;
            Session["QRCode02"] = list;
            js.code = 1;
            js.text = "Thành công";
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ShowInQRCode2(String SoEPI, string ID)
        {
            //var model = db.BD_InQRCode2(SoEPI).ToList();
            //string[] arrayID = ID.Split(';');
            //long lID = 0;
            //var data = new List<BD_InQRCode2_Result>();
            var data = (List<BD_InQRCode2_Result>)(Session["QRCode02"]);
            List<BD_InQRCode2_Result> dataIn = new List<BD_InQRCode2_Result>();
            foreach (var item in data)
            {
                //if (arrayID[i].Trim() == null || arrayID[i].Trim() == "")
                //    break;
                //lID = long.Parse(arrayID[i]);
                //var item = model.FirstOrDefault(it => it.IDLenhRaiHang == lID);
                for (int j = 0; j < item.SoLuongEPI; j++)
                {
                    dataIn.Add(new BD_InQRCode2_Result() { IDLenhRaiHang = item.IDLenhRaiHang, SoEPI = item.SoEPI, SoPO = item.SoPO,
                        MaSanPham = item.MaSanPham, MaChiTietSanPham = item.MaChiTietSanPham,
                        MoTa = (j + 1).ToString() + " OF " + item.SoLuongEPI.ToString(), KichThuocSanPham = item.KichThuocSanPham,
                        MaMauOrVai = item.MaMauOrVai,
                        ODM = item.ODM,
                        SoLuongEPI = item.SoLuongEPI,
                        Ngay = item.Ngay,
                        QRCode = item.IDLenhRaiHang.ToString() + "_" + (j + 1).ToString(),
                        MaKH = item.MaKH,
                        MaThung = item.MaThung,
                        TayNam = item.TayNam,
                        MauTrong = item.MauTrong,
                       LoaiDongGoi = item.LoaiDongGoi

                    });
                }
            }
            var reportModel = new ReportsModel();
            reportModel.ReportName = "InQRCode2";
            reportModel.Report = XtraReport.FromFile(Server.MapPath("~/Content/upload/reports/InQRCode2.repx"), true);
            reportModel.Report.DataSource = dataIn;
            return View(reportModel);
        }
        [RoleAuthorize(Roles = "0,3=6")]
        public async Task<FileResult> XuatFileMau()
        {
            string filename = Server.MapPath("~/Content/upload/FileMau/MauLenhRaiHang.xlsx");
            return File(filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MauLenhRaiHang.xlsx");
        }

        [RoleAuthorize(Roles = "0,24=1")]
        public async Task<ActionResult> InMauQRCode2TuChon()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,24=1")]
        public async Task<JsonResult> getDataInQRCoceTuChon(string SoEPI, string MaSKU)
        {
            var model = db.BD_InQRCode2TuChon(SoEPI, MaSKU).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        [RoleAuthorize(Roles = "0,24=1")]
        public async Task<JsonResult> AddSessionQR2(List<BD_InQRCode2TuChon_Result> list)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            Session["QRCode2"] = null;
            Session["QRCode2"] = list;
            js.code = 1;
            js.text = "Thành công";
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ShowInQRCode2TuChon()
        {
            var data = (List< BD_InQRCode2TuChon_Result>)(Session["QRCode2"]);
            List<BD_InQRCode2TuChon_Result> dataIn = new List<BD_InQRCode2TuChon_Result>();
            string[] sCanIn;
            string sLoi = "";
            int soLuong;
            foreach (var item in data)
            {
               
                if (item.SoCanIn.Length > 0)
                {
                    sCanIn = item.SoCanIn.Split(',');
                    for (int i = 0; i < sCanIn.Length; i++)
                    {
                        try
                        {
                            if (sCanIn[i] != null && sCanIn[i] != "")
                            {
                                soLuong = int.Parse(sCanIn[i]);
                                if (soLuong > item.SoLuongEPI)
                                {
                                    sLoi += "Lỗi số lượng in ở mã " + item.MaChiTietSanPham + "số lượng Num Of " + soLuong.ToString() + " Lớn hơn số trong EPI \n";
                                }
                                dataIn.Add(new BD_InQRCode2TuChon_Result()
                                {
                                    IDLenhRaiHang = item.IDLenhRaiHang,
                                    SoEPI = item.SoEPI,
                                    SoPO = item.SoPO,
                                    MaSanPham = item.MaSanPham,
                                    MaChiTietSanPham = item.MaChiTietSanPham,
                                    MoTa = (soLuong).ToString() + " OF " + item.SoLuongEPI.ToString(),
                                    KichThuocSanPham = item.KichThuocSanPham,
                                    MaMauOrVai = item.MaMauOrVai,
                                    ODM = item.ODM,
                                    SoLuongEPI = item.SoLuongEPI,
                                    Ngay = item.Ngay,
                                    QRCode = item.IDLenhRaiHang.ToString() + "_" + (soLuong).ToString(),
                                    MaKH = item.MaKH,
                                    MaThung = item.MaThung,
                                    TayNam = item.TayNam,
                                    MauTrong = item.MauTrong,
                                    LoaiDongGoi = item.LoaiDongGoi

                                });
                            }
                        }
                        catch (Exception)
                        {
                            sLoi += "Lỗi số lượng in ở mã " + item.MaChiTietSanPham + "số lượng in là " + item.SoCanIn + " \n";
                            continue;
                        }
                    }
                }
            }
            if (sLoi.Length > 0)
            {
                return Content(sLoi);
            }

            var reportModel = new ReportsModel();
            reportModel.ReportName = "InQRCode2";
            reportModel.Report = XtraReport.FromFile(Server.MapPath("~/Content/upload/reports/InQRCode2.repx"), true);
            reportModel.Report.DataSource = dataIn;
            return View(reportModel);
        }
        [RoleAuthorize(Roles = "0,19=1")]
        public async Task<ActionResult> LichSuQuetSanXuat()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,19=1")]
        public async Task<JsonResult> getLichSuQuetSanXuat(string MaCongDoan,string MaSKU,string SoEPI)
        {
            var data = db.BD_LichSuQuetSanXuat(SoEPI, MaSKU, MaCongDoan).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0")]
        public async Task<JsonResult> DelAll(String SoEPI)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            SoEPI = SoEPI.Trim();
            var checkXuatHang = db.BD_PhieuNhapHangChiTiet.Where(it => it.SoEPI == SoEPI.Trim()).ToList();
            if (checkXuatHang.Count > 0)
            {
                js.text = "Số EPI <b> " +SoEPI+" </b>đã xuất hàng trắng không thể xóa";
            }
            else
            {
                var del = db.BD_LenhRaiHang.Where(it => it.SoEPI == SoEPI).ToList();
                db.BD_LenhRaiHang.RemoveRange(del);
                if (db.SaveChanges() > 0)
                {
                    js.code = 1;
                    js.text = "Thành công";
                }
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
    }
    public class InQR2TuCHon {
        public int Id { get; set; }
        public string SoCanIn { get; set; }
    }
}