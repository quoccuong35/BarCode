using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using WMS.Models;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using System.Transactions;
using WMS.DB;
using DevExpress.XtraReports.UI;
namespace WMS.Controllers
{
    [RoleAuthorize(Roles = "0,1=1")]
    [Authorize]
    public class PurchaseOrderController : Controller
    {
        // GET: PurchaseOrder
        WMSEntities db = new WMSEntities();
        GridData grid = new GridData();
        [RoleAuthorize(Roles = "0,1=1")]
        public ActionResult Index()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,1=1")]
        public async Task<ActionResult> LocDanhSach(DateTime TuNgay, DateTime DenNgay, string SoHopDong, string SoPO, int? NhaCungCap)
        {
            ListPO model = new ListPO();

            //var po = db.BD_Po.Where(it => it.NgayGocPhatHanh >= TuNgay.Date && it.NgayGocPhatHanh <= DenNgay && it.Del != true).ToList();

            List< BDPO> po = (from a in db.BD_Po
                      where a.NgayGocPhatHanh >= TuNgay && a.NgayGocPhatHanh <= DenNgay && a.Del != true
                      select new BDPO {
                          IdNhaCungCap = a.IdNhaCungCap,
                          IDPO = a.IDPO,
                          SoPO = a.SoPO,
                          SoHopDong = a.SoHopDong,
                          DiaChi = a.DiaChi,
                          TenNhaSanXuat = a.TenNhaSanXuat,
                          SoLichGiaoHang = a.SoLichGiaoHang,
                          NgayGocPhatHanh = a.NgayGocPhatHanh,
                          MaNhaSanXuat = a.MaNhaSanXuat
                      }
                      ).ToList();
            po.All(c => { c.NgayHopDong = c.NgayGocPhatHanh.Value.ToString("dd/MM/yyyy"); return true; });
            if (SoPO != null && SoPO != "")
            {
                po = po.Where(it => it.SoPO.Contains(SoPO)).ToList();
            }
            if (SoHopDong != null && SoHopDong != "")
            {
                po = po.Where(it => it.SoHopDong.Contains(SoHopDong)).ToList();
            }
            if (NhaCungCap != null)
            {
                po = po.Where(it => it.IdNhaCungCap == NhaCungCap).ToList();
            }
            model.listPO = po;
            List<long> iPO = po.Select(it => it.IDPO).ToList();
            model.listPOChiTiet = db.BD_POChiTiet.Where(it => it.Del != true && iPO.Contains(it.IDPO)).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,1=1")]
        public async Task<ActionResult> PhieuNhapPO(int? id)
        {
            PO model = new PO();
            if (id != null)
            {

                model.ThongTinPO = db.BD_Po.FirstOrDefault(it => it.IDPO == id && it.Del != true);
                model.POChiTiet = db.BD_POChiTiet.Where(it => it.IDPO == id && it.Del != true).ToList();
            }
            else
            {
                BD_Po po = new BD_Po();
                po.IDPO = 0;
                po.NgayGocPhatHanh = DateTime.Now;

                List<BD_POChiTiet> poct = new List<BD_POChiTiet>();
                model.ThongTinPO = po;
                model.POChiTiet = poct;
            }

            return View(model);
        }
        //[RoleAuthorize(Roles = "0")]
        public async Task<JsonResult> EditChiTietPO(BD_POChiTiet item)
        {
            var model = db.BD_POChiTiet.FirstOrDefault(it => it.ID == item.ID);
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            string[] rs = { "0", "Thất bại" };
            if (!User.IsInRole("0") && User.Identity.Name != "pm.inven")
            {
                return Json(rs, JsonRequestBehavior.AllowGet);
            }
            if (item.SoLuongPO != null)
            {
                model.SoLuongPO = item.SoLuongPO;
            }
            if (item.DonViTinh != null)
            {
                model.DonViTinh = item.DonViTinh;
            }
            if (item.TenQuanLyKhoCIRS != null)
            {
                model.TenQuanLyKhoCIRS = item.TenQuanLyKhoCIRS;
            }
            if (item.TenChiTietSanPham != null)
            {
                model.TenChiTietSanPham = item.TenChiTietSanPham;
            }
            if (item.SoLuongPO != null)
            {
                model.SoLuongPO = item.SoLuongPO;
            }
            if (item.MaChiTietSanPham != null && User.IsInRole("0"))
            {
                model.MaChiTietSanPham = item.MaChiTietSanPham;
            }
            if (item.MaSanPham != null && User.IsInRole("0"))
            {
                model.MaSanPham = item.MaSanPham;
            }
            model.NguoiDungSua = nguoidung.IdNguoiDung;
            model.NgaySua = DateTime.Now;
            int i = db.SaveChanges();
            if (i > 0)
            {
                rs[0] = "1";
                rs[1] = "Thành công";
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [RoleAuthorize(Roles = "0,1=4")]
        public async Task<JsonResult> DelPOChiTiet(BD_POChiTiet item)
        {
            string[] rs = { "0", "Thất bại" };
            var checkDel = db.BD_PhieuNhapHangChiTiet.Where(it => it.IDPOChiTiet == item.ID).Distinct().ToList();
            if (checkDel.Count > 0)
            {
                rs[0] = "0";
                rs[1] = "Thông tin WSO đã nhập kho không thể xóa";
                return Json(rs, JsonRequestBehavior.AllowGet);
            }
            var model = db.BD_POChiTiet.FirstOrDefault(it => it.ID == item.ID);

      
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            
            if (model != null)
            {
                model.Del = true;
                model.NguoiDungSua = nguoidung.IdNguoiDung;
                model.NgaySua = DateTime.Now;
                int i = db.SaveChanges();
                if (i > 0)
                {
                    rs[0] = "1";
                    rs[1] = "Thành công";
                }
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,1=2")]
        public async Task<JsonResult> LayFileExcel(HttpPostedFileBase importFile)
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

                    List<BD_POChiTiet> modle = getDataTableFromExcel(resultFilePath, ref sLoi);

                    if (sLoi.Length > 0)
                    {
                        st.code = 0;
                        st.text = sLoi;
                    }
                    else
                    {
                        st.code = 1;
                        st.data = modle;
                    }
                }
                return Json(st);
            }
            catch (Exception)
            {
                return Json(st);
            }
        }
        public List<BD_POChiTiet> getDataTableFromExcel(string path, ref string sLoi)
        {
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            List<BD_POChiTiet> data = new List<BD_POChiTiet>();
            using (XLWorkbook workBook = new XLWorkbook(path)) {
                IXLWorksheet workSheet = workBook.Worksheet(1);
                bool firstRow = true;
                DataTable dt = new DataTable();
                int i = 0;
                string MaChiTiet = "",MaSanPham = "";
                foreach (IXLRow row in workSheet.Rows())
                {
                    MaChiTiet = row.Cell(3).Value.ToString();
                    MaSanPham = row.Cell(2).Value.ToString();
                    if (MaChiTiet == "")
                        break;
                    i++;
                    var checkItem = data.Where(it => it.MaChiTietSanPham.Contains(MaChiTiet) && it.MaSanPham.Contains(MaSanPham)).ToList();
                    if (checkItem.Count > 0)
                    {
                        sLoi += "Mã chi tiết dòng thứ " + i.ToString() + " đã tồn tại";
                    }
                    if (firstRow)
                    {
                        firstRow = false;
                        continue;
                    }
                    else
                    {
                        BD_POChiTiet add = new BD_POChiTiet();
                        add.ID = 0;
                        add.MaSanPham = MaSanPham;
                        add.MaChiTietSanPham = row.Cell(3).Value.ToString();
                        add.TenChiTietSanPham = row.Cell(4).Value.ToString();
                        add.TenQuanLyKhoCIRS = row.Cell(5).Value.ToString();
                        add.DonViTinh = row.Cell(6).Value.ToString();
                        add.SoLuongTrongMoiBo = row.Cell(7).Value.ToString();
                        try
                        {
                            add.SoLuongPO = int.Parse(row.Cell(8).Value.ToString().Trim());
                        }
                        catch
                        {
                            sLoi += " Lỗi số lượng trong PO dồng " + i.ToString();
                        }
                        add.NguoiDungTao = nguoidung.IdNguoiDung;
                        add.NgayTao = DateTime.Now;
                        data.Add(add);
                    }
                }
                return data;
            }

        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,1=2")]
        public async Task<JsonResult> ThemPO(BD_Po item, List<BD_POChiTiet> columns)
        {
            JsonStatus rs = new JsonStatus();
            rs.text = "Thất bại";
            rs.code = 0;
            if (await CheckPOTonTai(0, item.SoPO))
            {
                rs.code = 0;
                rs.text = "Số PO đã tồn tại";
                return Json(rs);
            }
            if (await CheckSoHopDongTonTai(0, item.SoHopDong.Trim()))
            {
                rs.code = 0;
                rs.text = "Số hợp đồng đã tồn tại";
                return Json(rs);
            }


            using (var tran = new TransactionScope())
            {
                try
                {
                    var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                    var NhaCungCap = db.HS_NhaCungCap.FirstOrDefault(it => it.IdNhaCungCap == item.IdNhaCungCap);
                    if (NhaCungCap != null)
                    {
                        item.TenNhaSanXuat = NhaCungCap.TenNhaCungCap;
                        item.DiaChi = NhaCungCap.DiaChi;
                        item.MaNhaSanXuat = NhaCungCap.MaNhaCungCap;
                        item.NguoiDungTao = nguoidung.IdNguoiDung;
                        item.Del = false;
                        item.NgayTao = DateTime.Now;

                    }
                    db.BD_Po.Add(item);
                    int sr = db.SaveChanges();
                    if (sr > 0)
                    {
                        long? IDPO;
                        IDPO = db.BD_Po.Where(it => it.SoPO == item.SoPO && it.Del != true && it.NguoiDungTao == nguoidung.IdNguoiDung).Max(it=>it.IDPO);
                        if (IDPO != null)
                        {
                            
                            columns.All(c => {
                                c.IDPO = IDPO.Value; c.SoPO = item.SoPO; c.SoLichGiaoHang = item.SoLichGiaoHang;
                                c.NguoiDungTao = nguoidung.IdNguoiDung; c.NgayTao = DateTime.Now; c.SoLuongNhap = 0; c.Del = false; return true;
                            });
                            db.BD_POChiTiet.AddRange(columns);
                            if (db.SaveChanges() > 0)
                            {
                                tran.Complete();//cập nhật thành công
                                rs.code = 1;
                                rs.text = "Thành công";

                            }
                        }
                        
                    }
                    return Json(rs);

                }
                catch (Exception ex)
                {
                    rs.text = ex.Message;
                    return Json(rs);
                }
            }

        }
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,1=4")]
        public async Task<JsonResult> DelPO(int IDPO)
        {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            var del = db.BD_Po.FirstOrDefault(it => it.IDPO == IDPO);
            var checkData = (from a in db.BD_POChiTiet join
                            b in db.BD_PhieuNhapHangChiTiet on a.ID equals b.IDPOChiTiet
                             where a.IDPO == IDPO
                             select a).Distinct().ToList();
            if (checkData.Count > 0 )
            {
                rs.code = 0;
                rs.text = "Thông tin WSO đã nhập hàng không thể xóa";
                return Json(rs);
            }
            if (del != null)
            {
                del.Del = true;
                del.NguoiDungSua = nguoidung.IdNguoiDung;
                del.NgaySua = DateTime.Now;

                var listPO = db.BD_POChiTiet.Where(it => it.IDPO == IDPO).ToList();
                listPO.All(c => { c.Del = true; c.NguoiDungSua = nguoidung.IdNguoiDung; c.NgaySua = DateTime.Now; return true; });
                db.SaveChanges();
                rs.code = 1;
                rs.text = "Thành công";
            }
            return Json(rs);
        }
        public async Task<JsonResult> ChecPO(int id, string SoPhieu)
        {

            return Json(await CheckPOTonTai(id, SoPhieu), JsonRequestBehavior.AllowGet);
        }
        public async Task<bool> CheckPOTonTai(int id, string SoPhieu)
        {
            bool is_exit = false;
            if ((id == 0 && await db.BD_Po.CountAsync(m => m.SoPO == SoPhieu && m.Del != true) == 0) || (id > 0 && await db.BD_Po.CountAsync(m => m.SoPO == SoPhieu && m.IDPO != id && m.Del != true) == 0))
            {
                is_exit = false;
            }
            else
            {
                is_exit = true;
            }
            return is_exit;
        }

        public async Task<JsonResult> ChecSoHopDong(int id, string SoHD)
        {
            return Json(await CheckSoHopDongTonTai(id, SoHD), JsonRequestBehavior.AllowGet);
        }
        public async Task<bool> CheckSoHopDongTonTai(int id, string SoHD)
        {
            bool is_exit = false;
            if ((id == 0 && await db.BD_Po.CountAsync(m => m.SoHopDong == SoHD && m.Del != true) == 0) || (id > 0 && await db.BD_Po.CountAsync(m => m.SoHopDong == SoHD && m.IDPO != id && m.Del != true) == 0))
            {
                is_exit = false;
            }
            else
            {
                is_exit = true;
            }
            return is_exit;
        }
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,1=3")]
        public async Task<JsonResult> ThemPOChiTiet(BD_POChiTiet item) {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            item.NguoiDungTao = nguoidung.IdNguoiDung;
            item.SoLuongNhap = 0;
            item.Del = false;
            item.NgayTao = DateTime.Now;
            db.BD_POChiTiet.Add(item);
            if (db.SaveChanges() > 0)
            {
                rs.code = 1;
                rs.text = "Thành công";
                var model = db.BD_POChiTiet.Where(it => it.IDPO == item.IDPO && it.Del != true).ToList().OrderByDescending(it => it.ID);
                rs.data = model;
            }

            return Json(rs);
        }

        public async Task<JsonResult> ChecMaChiTiet(int idPO, int? id, string MaChiTietSanPham)
        {
            return Json(await CheckMaChiTiet(idPO, id, MaChiTietSanPham), JsonRequestBehavior.AllowGet);
        }
        public async Task<bool> CheckMaChiTiet(int idPO,int? id, string MaChiTietSanPham)
        {
            bool is_exit = true;
            if ( (id== null && await db.BD_POChiTiet.CountAsync(m => m.MaChiTietSanPham == MaChiTietSanPham && m.IDPO == idPO && m.Del !=true )>0)|| (id != null && idPO > 0 && await db.BD_POChiTiet.CountAsync(m => m.MaChiTietSanPham == MaChiTietSanPham && m.IDPO == idPO && m.ID != id && m.Del!= true) > 0))
            {
                is_exit = false;
            }
           
            return is_exit ;
        }

        public async Task<FileResult> XuatFileMau()
        {
            string filename = Server.MapPath("~/Content/upload/FileMau/MauInPortPoChiTiet.xlsx");
            return File(filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MauInPortPoChiTiet.xlsx");
        }

        public async Task<FileResult> XuatFileExcel(DateTime TuNgay, DateTime DenNgay, string SoHopDong, string SoPO, int? NhaCungCap)
        {
            using (XLWorkbook wb = new XLWorkbook(Server.MapPath("~/Content/upload/FileMau/FileExportPO.xlsx")))
            {
                var model = (from po in db.BD_Po
                             join poct in db.BD_POChiTiet on po.IDPO equals poct.IDPO
                             where po.NgayGocPhatHanh >= TuNgay
                                   && po.NgayGocPhatHanh <= DenNgay
                             select new
                             {
                                 po.MaNhaSanXuat,
                                 po.TenNhaSanXuat,
                                 po.DiaChi,
                                 po.NgayGocPhatHanh,
                                 po.SoHopDong,
                                 po.SoPO,
                                 po.SoLichGiaoHang,
                                 poct.MaSanPham,
                                 poct.MaChiTietSanPham,
                                 poct.TenChiTietSanPham,
                                 poct.TenQuanLyKhoCIRS,
                                 poct.SoLuongTrongMoiBo,
                                 poct.DonViTinh,
                                 poct.SoLuongPO
                             }).ToList();
                IXLWorksheet FromBaoCao = wb.Worksheet(1);
                
                FromBaoCao.Cell(2, 1).InsertData(model);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","FileWSOChiTiet.xlsx");
                }
            }
        }
        [HttpGet]
        [RoleAuthorize(Roles = "0,1=6")]
        public async Task<ActionResult> InQRCode1(){
            return View();
        }
        [RoleAuthorize(Roles = "0,1=6")]
        public async Task<JsonResult> getDataInQRCoce(string SoPO)
        {
            var model = db.BD_InQRCode1(SoPO).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
            //JsonStatus st = new JsonStatus();
            //try
            //{
            //    st.data = grid.GetGridData("BD_InQRCode1", Page, PageSize, Filter, isLoadingAll ? 1 : 0);
            //    st.code = 1;
            //}
            //catch (Exception ex)
            //{
            //    st.code = 0;
            //    st.description = ex.Message;
            //}
            //var json = Json(st, JsonRequestBehavior.AllowGet);
            //json.MaxJsonLength = int.MaxValue;
            //return json;
        }
        [RoleAuthorize(Roles = "0,1=6")]
        public async Task<JsonResult> AddSession(List<BD_InQRCode1_Result> list)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            Session["QRCode01"] = null;
            Session["QRCod01"] = list;
            js.code = 1;
            js.text = "Thành công";
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ShowInQRCode1()
        {
            //var model = db.BD_InQRCode1(SoPO).ToList();
            //string[] arrayID = ID.Split(';');
            //string[] arraySoLuongBD = SoLuongBD.Split(';');
            //string[] arraySoLuongIn = SoLuongIn.Split(';');
            int iSoLuongBD = 0, iSoLuongIn = 0; long lID = 0;
            //List<BD_InQRCode1_Result> data = new List<BD_InQRCode1_Result>();
            var QuyCach = db.Sys_CauHinh.FirstOrDefault(it => it.TuKhoa == "KyHieuQRCode1");
            //for (int i = 0; i < arrayID.Length; i++)
            //{
            //    if (arraySoLuongBD[i].Trim() == null || arraySoLuongBD[i].Trim() == "")
            //        break;
            //    iSoLuongBD = int.Parse(arraySoLuongBD[i].Trim())+1;
            //    iSoLuongIn = int.Parse(arraySoLuongIn[i].Trim())+iSoLuongBD - 1;
            //    lID = long.Parse(arrayID[i]);
            //    var item = model.FirstOrDefault(it => it.ID == lID);
            //    //data.Add(item);
            //    for (int j = iSoLuongBD; j <= iSoLuongIn; j++)
            //    {
            //        data.Add(new BD_InQRCode1_Result {MaChiTietSanPham = item.MaChiTietSanPham,QRCode = QuyCach.GiaTri.ToString().Trim()+ item.ID.ToString()+"_"+j.ToString()});
            //    }
            //}
            List<BD_InQRCode1_Result> dataIn = new List<BD_InQRCode1_Result>();
            var dataSession = (List<BD_InQRCode1_Result>)(Session["QRCod01"]);
            List<BD_LichSuInQRCode1> addLichSu = new List<BD_LichSuInQRCode1>();
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            List<long> id = dataSession.Select(it => it.ID).ToList();
            //var kiemTraDaIn = db.BD_LichSuInQRCode1.Where(it => id.Contains(it.IDPoChiTiet)).ToList();
            string sLoi = "";
            foreach (var item in dataSession)
            {
                iSoLuongBD = iSoLuongIn = 0;
                iSoLuongBD = item.SoDaIn + 1;
                iSoLuongIn = item.SoLuongCanIn.Value + iSoLuongBD - 1;
                //var check = kiemTraDaIn.Where(it => it.TuSo <= iSoLuongBD && iSoLuongBD <= it.DenSo && it.IDPoChiTiet == item.ID).ToList();
                //var check1 = kiemTraDaIn.Where(it=>it.DenSo >= iSoLuongIn && it.IDPoChiTiet == item.ID).ToList();
                //if (check.Count > 0 || check1.Count > 0)
                //{
                //    sLoi += "Tồn tại số OF đã in trong mã SKU <b>" + item.MaChiTietSanPham + "</b> không thể in lại </br>";
                //}
                iSoLuongIn = item.SoLuongCanIn == null ? 0 : item.SoLuongCanIn.Value;
                if (iSoLuongIn == 0)
                    continue;
                addLichSu.Add(new BD_LichSuInQRCode1 { IDPoChiTiet = item.ID, TuSo = iSoLuongBD, DenSo = iSoLuongIn, NgayIn = DateTime.Now, NguoiDungIn = nguoidung.IdNguoiDung,MaSKU = item.MaChiTietSanPham });
                db.Database.CommandTimeout = 600;
                var numOf = db.bd_InQRCode1SoOF(item.ID, item.TongSo, item.SoLuongCanIn.Value).ToList();
                foreach (var num in numOf)
                {
                    dataIn.Add(new BD_InQRCode1_Result { MaChiTietSanPham = item.MaChiTietSanPham, QRCode = QuyCach.GiaTri.ToString().Trim() + item.ID.ToString() + "_" + num.ToString(), TenNhaCungCap = item.SoPO + "-" + num.ToString() + "/" + item.TongSo.ToString() });
                }
                //for (int j = iSoLuongBD; j <= iSoLuongIn; j++)
                //{
                //    dataIn.Add(new BD_InQRCode1_Result { MaChiTietSanPham = item.MaChiTietSanPham, QRCode = QuyCach.GiaTri.ToString().Trim() + item.ID.ToString() + "_" + j.ToString(),TenNhaCungCap = item.SoPO+"-"+j.ToString()+"/"+item.TongSo.ToString() });
                //}
            }
            if (sLoi.Length > 0)
            {
                return Content(MvcHtmlString.Create(sLoi).ToHtmlString());
            }
            else
            {
                if (addLichSu.Count > 0)
                {
                    db.BD_LichSuInQRCode1.AddRange(addLichSu);
                    if (db.SaveChanges() > 0)
                    {
                        var reportModel = new ReportsModel();
                        reportModel.ReportName = "InQRCode1";
                        reportModel.Report = XtraReport.FromFile(Server.MapPath("~/Content/upload/reports/InQRCode1.repx"), true);
                        reportModel.Report.DataSource = dataIn;
                        return View(reportModel);
                    }
                    else
                    {
                        return Content("Lỗi hệ thống liên hệ nhà quản lý để đc hổ trợ");
                    }
                }
                else
                {
                    return Content("Không có dữ liệu in");
                }
               
            }
           
           
        }

        [HttpGet]
        [RoleAuthorize(Roles = "0,23=1")]
        public async Task<ActionResult> InQRCode1TuChon()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,1=6,23=1")]
        public async Task<JsonResult> getDataInQRCoceTuChon(string SoPO,string MaSKU)
        {
            var model = db.BD_InQRCode1TuChon(SoPO, MaSKU).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
            //JsonStatus st = new JsonStatus();
            //try
            //{
            //    st.data = grid.GetGridData("BD_InQRCode1", Page, PageSize, Filter, isLoadingAll ? 1 : 0);
            //    st.code = 1;
            //}
            //catch (Exception ex)
            //{
            //    st.code = 0;
            //    st.description = ex.Message;
            //}
            //var json = Json(st, JsonRequestBehavior.AllowGet);
            //json.MaxJsonLength = int.MaxValue;
            //return json;
        }
        [RoleAuthorize(Roles = "0,1=6,23=1")]
        public async Task<JsonResult> AddSessionQR1(List<BD_InQRCode1TuChon_Result> list)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            Session["QRCode1"] = null;
            Session["QRCode1"] = list;
            js.code = 1;
            js.text = "Thành công";
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ShowInQRCode1TuChon()
        {
            var data = (List<BD_InQRCode1TuChon_Result>)(Session["QRCode1"]);
            List<BD_InQRCode1TuChon_Result> dataIn = new List<BD_InQRCode1TuChon_Result>();
            string[] sCanIn;
            string sLoi = "";
            int soLuong;
            var QuyCach = db.Sys_CauHinh.FirstOrDefault(it => it.TuKhoa == "KyHieuQRCode1");
            foreach (var item in data)
            {

                if (item.QRCode!= null && item.QRCode.Length > 0)
                {
                    sCanIn = item.QRCode.Split(',');
                    for (int i = 0; i < sCanIn.Length; i++)
                    {
                        try
                        {
                            if (sCanIn[i] != null && sCanIn[i] != "")
                            {
                                soLuong = int.Parse(sCanIn[i]);
                                if (soLuong > item.TongSo)
                                {
                                    sLoi += "Lỗi số lượng in ở mã " + item.MaChiTietSanPham + "số lượng Num Of " + soLuong.ToString() + " Lớn hơn số trong số WSO/PO \n";
                                }
                                dataIn.Add(new BD_InQRCode1TuChon_Result { MaChiTietSanPham = item.MaChiTietSanPham, QRCode = QuyCach.GiaTri.ToString().Trim() + item.ID.ToString() + "_" + soLuong.ToString(), TenNhaCungCap = item.SoPO + "-" + soLuong.ToString() + "/" + item.TongSo.ToString() });
                            }
                        }
                        catch (Exception)
                        {
                            sLoi += "Lỗi số lượng in ở mã " + item.MaChiTietSanPham + " số lượng in là " + item.QRCode + " \n";
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
            reportModel.ReportName = "InQRCode1";
            reportModel.Report = XtraReport.FromFile(Server.MapPath("~/Content/upload/reports/InQRCode1.repx"), true);
            reportModel.Report.DataSource = dataIn;
            return View(reportModel);
        }
    }
}