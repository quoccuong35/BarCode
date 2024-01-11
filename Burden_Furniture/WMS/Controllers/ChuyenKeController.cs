using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Libs;
using WMS.Models;
using System.IO;
using System.Data;
using System.Web.Mvc;
using WMS.DB;
using System.Transactions;



namespace WMS.Controllers
{
    //[RoleAuthorize(Roles = "0,4=1,18=1")]
    public class ChuyenKeController : Controller
    {
      
        // GET: ChuyenKe
        WMSEntities db = new WMSEntities();
        GridData grid = new GridData();

        [RoleAuthorize(Roles = "0,4=3")]
        public async Task<ActionResult> Index()
        {
            return View();
        }
        public async Task<JsonResult> getHangTrangTheoKe(string MaKe,string MaChiTiet)
        {
            //JsonStatus st = new JsonStatus();
            //try
            //{
            //    st.data = grid.GetGridData("BDChuyenKe", Page, PageSize, Filter, isLoadingAll ? 1 : 0);
            //    st.code = 1;
            //}
            //catch (Exception ex)
            //{
            //    st.code = 0;
            //    st.description = ex.Message;
            //}
            //var json = Json(st, JsonRequestBehavior.AllowGet);
            //json.MaxJsonLength = int.MaxValue;
            //return Json(st, JsonRequestBehavior.AllowGet);
            var model = db.BD_ChuyenKe(MaKe).ToList();
            if (MaChiTiet != null && MaChiTiet.Length > 0)
            {
                model = model.Where(it => it.MaChiTietSanPham == MaChiTiet.Trim()).ToList();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //public async Task<>
        [RoleAuthorize(Roles = "0,4=3")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CapNhatChuyenKe(List<BD_ChuyenKe_Result> list)
        {
            JsonStatus rs = new JsonStatus();
            rs.text = "Thất bại";
            rs.code = 0;
            var listKe = db.HS_Ke.ToList();
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);

            using (var tran = new TransactionScope())
            {
                try
                {
                    foreach (var item in list)
                    {
                        var checkKe = listKe.FirstOrDefault(it => it.IdKe == item.MaKeChuyen.Value);
                        var listUpDate = db.BD_PhieuNhapHangChiTiet.Where(it => it.IdKe == item.IdKe && it.XuatKho != true && it.MaChiTietSanPham == item.MaChiTietSanPham).Take(item.SoLuongChuyen.Value).ToList();

                        foreach (var lichsu in listUpDate)
                        {
                            var add = new BD_LichSuChuyenKe();
                            add.IDPhieuNhapHangChiTiet = lichsu.IDPhieuNhapHangChiTiet;
                            add.IdKeCu = item.IdKe.Value;
                            add.IdKeMoi = item.MaKeChuyen.Value;
                            add.SoLuong = 1;
                            add.QRCode = lichsu.QRCode;
                            add.NguoiDungChuyen = nguoidung.IdNguoiDung;
                            add.NgayChuyen = DateTime.Now;
                            db.BD_LichSuChuyenKe.Add(add);
                        }
                        listUpDate.All(c => {
                            c.IdKe = checkKe.IdKe; c.MaKe = checkKe.MaKe; c.NguoiDungSua = nguoidung.IdNguoiDung; c.NgaySua = DateTime.Now; return true;
                        });
                        db.SaveChanges();
                    }
                  
                    tran.Complete();
                    rs.code = 1;
                    rs.text = "Thành công";
                    tran.Dispose();
                }
                catch
                {
                    tran.Dispose();
                }
            }
            return Json(rs,JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,4=1")]
        public async Task<ActionResult> LichSuChuyenKe() {
            return View();
        }
        public async Task<JsonResult> getLichSuChuyenKe(DateTime TuNgay, DateTime DenNgay, int? NguoiChuyen,string MaSKU,string MaSP)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            var model = db.PP_LichSuChyenKe(TuNgay, DenNgay, NguoiChuyen,MaSKU,MaSP).ToList();
            if (NguoiChuyen != null)
            {
                model = model.Where(it => it.NguoiDungChuyen == NguoiChuyen).ToList();
            }
            js.data = model;
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,18=1")]
        public async Task<ActionResult> LichSuChuyenKeThanhPham()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,18=1")]
        public async Task<JsonResult> getLichSuChuyenKeThanhPham(DateTime TuNgay, DateTime DenNgay, int? NguoiChuyen,string MaSKU,string MaSP)
        {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            var model = db.BD_LichSuChyenKeThanhPham(TuNgay, DenNgay, NguoiChuyen, MaSKU, MaSP).ToList();
            if (NguoiChuyen != null)
            {
                model = model.Where(it => it.NguoiDungChuyen == NguoiChuyen).ToList();
            }
            js.data = model;
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,18=3")]
        public async Task<ActionResult> ChuyenKeThanhPham()
        {
            return View();
        }
        public async Task<JsonResult> getThanhPhamTheoKe(string MaKe, string MaChiTiet)
        {
            var model = db.BD_ChuyenKeThanhPham(MaKe,MaChiTiet).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //public async Task<>
        [RoleAuthorize(Roles = "0,18=3")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CapNhatChuyenKeThanhPham(List<BD_ChuyenKeThanhPham_Result> list)
        {
            JsonStatus rs = new JsonStatus();
            rs.text = "Thất bại";
            rs.code = 0;
            var listKe = db.HS_Ke.ToList();
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);

            using (var tran = new TransactionScope())
            {
                try
                {
                    foreach (var item in list)
                    {
                        var checkKe = listKe.FirstOrDefault(it => it.IdKe == item.MaKeChuyen.Value);
                        //   var listUpDate = db.BD_NhapKhoThanhPham.Where(it => it.IdKe == item.IdKe && it.XuatKho != true && it.MaChiTietSanPham == item.MaChiTietSanPham).Take(item.SoLuongChuyen.Value).ToList();
                        var listUpDate = (from a in db.BD_NhapKhoThanhPham
                                          join b in db.BD_LenhRaiHang on a.IDLenhRaiHang equals b.IDLenhRaiHang
                                          where a.IdKe == item.IdKe && b.MaChiTietSanPham == item.MaChiTietSanPham
                                          select new { a.Id, a.IdKe, a.QRCode,a.SoNumOf,b.IDLenhRaiHang }).Take(item.SoLuongChuyen.Value).ToList();
                        foreach (var lichsu in listUpDate)
                        {
                            var add = new BD_LichSuChuyenKeThanhPham();
                            add.IdNhapKhoThanhPham = lichsu.Id;
                            add.IdKeCu = item.IdKe;
                            add.IdKeMoi = item.MaKeChuyen.Value;
                            add.IDLenhRaiHang = lichsu.IDLenhRaiHang;
                            add.QRCode = lichsu.QRCode;
                            add.NguoiDungChuyen = nguoidung.IdNguoiDung;
                            add.NgayChuyen = DateTime.Now;
                            add.SoNumOf = lichsu.SoNumOf;
                            db.BD_LichSuChuyenKeThanhPham.Add(add);
                        }
                        var arrIdKe = listUpDate.Select(it => it.Id).ToList();
                        var UpKeThanhPham = db.BD_NhapKhoThanhPham.Where(it => arrIdKe.Contains(it.Id)).ToList();
                        UpKeThanhPham.All(c =>
                        {
                            c.IdKe = checkKe.IdKe; return true;
                        });
                        db.SaveChanges();
                    }

                    tran.Complete();
                    rs.code = 1;
                    rs.text = "Thành công";
                    tran.Dispose();
                }
                catch
                {
                    tran.Dispose();
                }
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
    }
}