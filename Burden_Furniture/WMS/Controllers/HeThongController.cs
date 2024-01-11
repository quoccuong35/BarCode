using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WMS.Models;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Libs;
using WMS.DB;
using Helpers;
using System.Data.Entity.Validation;

namespace WMS.Controllers
{
    [RoleAuthorize(Roles = "0,9=1,10=1,11=1,12=1")]
    public class HeThongController : Controller
    {
        WMSEntities db = new WMSEntities();
        // GET: HeThong

            //Quản lý người dùng
        [HttpGet]
        [RoleAuthorize(Roles = "0,9=1")]
        public async Task<ActionResult>  QuanLyNguoiDung()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0")]
        public async Task<JsonResult> getNguoiDung() {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";

            var model = (from a in db.HT_NguoiDung
                         join b in db.HT_NhomNguoiDung on a.IdNhom equals b.IdNhom
                         where a.Xoa != true
                         && a.IdNguoiDung>1
                         select new { a.TaiKhoan, a.HoTen, a.HoatDong, b.TenNhom ,a.IdNguoiDung}
                        ).ToList();
            js.data = model;
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,9=1")]
        public async Task<ActionResult> NguoiDung(int? id)
        {
            HT_NguoiDung model = new HT_NguoiDung();
            if (id != null)
            {
                model = db.HT_NguoiDung.FirstOrDefault(it => it.IdNguoiDung == id);
                model.MatKhau = Helpers.Encode_Decode.Decrypt(model.MatKhau);
            }
            else
            {
                model.IdNguoiDung = 0;
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,9=2")]
        public async Task<JsonResult> AddNguoiDung(HT_NguoiDung item) {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            try
            {
             
                var check = db.HT_NguoiDung.FirstOrDefault(it => it.TaiKhoan == item.TaiKhoan);
                if (check != null)
                {
                    rs.text = "Tài khoản đã tồn tại không thể thêm";
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
                string MatKhau = Helpers.Encode_Decode.Encrypt(item.MatKhau);
                var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                item.MatKhau = MatKhau;
                item.NguoiTao = nguoidung.IdNguoiDung;
                item.NgayTao = DateTime.Now;
                item.HoatDong = true;
                item.Xoa = false;
                db.HT_NguoiDung.Add(item);
                if (db.SaveChanges() > 0)
                {
                    rs.code = 1;
                    rs.text = "Thành công";
                }
               
            }
            catch (DbEntityValidationException ex)
            {
                string s = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    s +=("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:"+
                        eve.Entry.Entity.GetType().Name+ eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        s += ve.PropertyName + "------"+ ve.ErrorMessage;
                    }
                    s += "/n";
                }
                rs.code = 0;
                rs.text = s;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,9=4")]
        public async Task<JsonResult> XoaNguoiDung( List<int> id ) {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            var model = db.HT_NguoiDung.Where(it => id.Contains(it.IdNguoiDung)).ToList();
            model.All(c => { c.Xoa = true; return true; });
            if (db.SaveChanges() > 0)
            {
                rs.code = 1;
                rs.text = "Thành công";
            }
            return Json(rs,JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,9=3")]
        public async Task<JsonResult> EditNguoiDung(HT_NguoiDung item) {
            string MatKhau = Helpers.Encode_Decode.Encrypt(item.MatKhau);
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            var edit = db.HT_NguoiDung.FirstOrDefault(it => it.IdNguoiDung == item.IdNguoiDung);
            edit.HoatDong = item.HoatDong;
            edit.HoTen = item.HoTen;
            edit.MatKhau = MatKhau;
            edit.IdNhom = item.IdNhom;
            edit.NgaySua = DateTime.Now;
            edit.NguoiSua = nguoidung.IdNguoiDung;
            if (db.SaveChanges() > 0)
            {
                rs.code = 1;
                rs.text = "Thành công";
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        // Doi mật khẩu ở người dùng
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LuuDoiMatKhau(string MatKhau, string TaiKhoan)
        {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            var model = db.HT_NguoiDung.FirstOrDefault(it => it.TaiKhoan == TaiKhoan.Trim());
            MatKhau = Helpers.Encode_Decode.Encrypt(MatKhau);
            model.MatKhau = MatKhau;
            if (db.SaveChanges() > 0)
            {
                rs.code = 1;
                rs.text = "Thành công";
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0")]
        public async Task<JsonResult> GetNhomNguoiDung() {
            JsonStatus rs = new JsonStatus();
            var model = db.HT_NhomNguoiDung.ToList();
            rs.data = model;
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,11=1")]
        public async Task<ActionResult> DMKe()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,11=1")]
        public async Task<JsonResult> GetKe() {
            JsonStatus rs = new JsonStatus();
            var data = db.HS_Ke.Where(it=>it.Del !=true).Select(it=>new { it.IdKe,it.TenKe,it.MaKe,QRCoe = "Ke_"+ it.IdKe});
            rs.data = data;
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,11=4")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DelKe(int id) {
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            var del = db.HS_Ke.FirstOrDefault(it => it.IdKe == id);
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            del.Del = true;
            del.NguoiDungSua = nguoidung.IdNguoiDung;
            del.NgaySua = DateTime.Now;
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,11=2")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddKe(HS_Ke item)
        {
           
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus js = new JsonStatus();
            var checkMaKe = db.HS_Ke.Where(it => it.MaKe == item.MaKe).ToList();
            if (checkMaKe.Count > 0)
            {
                js.text = "Tồn tại mã kệ trong hệ thống không thể thêm";
                js.code = 0;
                return Json(js, JsonRequestBehavior.AllowGet);
            }
           
            item.NguoiDungTao = nguoidung.IdNguoiDung;
            item.NgayTao = DateTime.Now;
            js.code = 0;
            js.text = "Thất bại";
            db.HS_Ke.Add(item);
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }

        [RoleAuthorize(Roles = "0,11=3")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditKe(int id,HS_Ke item)
        {
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";

            var checkMaKe = db.HS_Ke.Where(it => it.MaKe == item.MaKe.ToUpper().Trim() && it.IdKe != item.IdKe).ToList();
            if (checkMaKe.Count > 0)
            {
                js.text = "Tồn tại mã kệ trong hệ thống không thể sửa";
                js.code = 0;
                return Json(js, JsonRequestBehavior.AllowGet);
            }

            var model = db.HS_Ke.FirstOrDefault(it => it.IdKe == id);
            if(item.MaKe != null && item.MaKe != "")
                model.MaKe = item.MaKe.ToUpper();
            if(item.TenKe != null && item.TenKe != "")
                model.TenKe = item.TenKe;
            model.NguoiDungSua = nguoidung.IdNguoiDung;
            model.NgaySua = DateTime.Now;
            //model.ma
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        /// danh muc nhà cung cấp
        [RoleAuthorize(Roles = "0,12=1")]
        public async Task<ActionResult> DMNhaCungCap()
        {
            return View();
        }
        [RoleAuthorize(Roles = "0,12=1")]
        public async Task<JsonResult> GetNhaCungCap()
        {
            JsonStatus rs = new JsonStatus();
            var data = db.HS_NhaCungCap.Where(it => it.HienThi == true).ToList();
            rs.data = data;
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,12=2")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddNhaCungCap(HS_NhaCungCap item)
        {

            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus js = new JsonStatus();
            var checkMaKe = db.HS_NhaCungCap.Where(it => it.MaNhaCungCap == item.MaNhaCungCap).ToList();
            if (checkMaKe.Count > 0)
            {
                js.text = "Tồn tại mã nhà cung cấp trong hệ thống không thể thêm";
                js.code = 0;
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            item.HienThi = true;
            item.NguoiDungTao = nguoidung.IdNguoiDung;
            item.NgayTao = DateTime.Now;
            js.code = 0;
            js.text = "Thất bại";
            db.HS_NhaCungCap.Add(item);
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,12=3")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditNhaCungCap(int id, HS_NhaCungCap item)
        {
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";

            //var checkMaKe = db.HS_NhaCungCap.Where(it => it.MaNhaCungCap == item.MaNhaCungCap.ToUpper().Trim() && it.IdNhaCungCap != item.IdNhaCungCap).ToList();
            //if (checkMaKe.Count > 0)
            //{
            //    js.text = "Tồn tại mã nhà cung cấp trong hệ thống không thể sửa";
            //    js.code = 0;
            //    return Json(js, JsonRequestBehavior.AllowGet);
            //}

            var model = db.HS_NhaCungCap.FirstOrDefault(it => it.IdNhaCungCap == id);
            if (item.MaNhaCungCap != null && item.MaNhaCungCap != "")
                model.MaNhaCungCap = item.MaNhaCungCap.ToUpper();
            if (item.TenNhaCungCap != null && item.TenNhaCungCap != "")
                model.TenNhaCungCap = item.TenNhaCungCap;
            if (item.DiaChi != null && item.DiaChi != "")
                model.DiaChi = item.DiaChi;
            model.NguoiDungSua = nguoidung.IdNguoiDung;
            model.NgaySua = DateTime.Now;
            //model.ma
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,12=4")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DelNhaCungCap(int id)
        {
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            var del = db.HS_NhaCungCap.FirstOrDefault(it => it.IdNhaCungCap == id);
            del.HienThi = false;
            del.NguoiDungSua = nguoidung.IdNguoiDung;
            del.NgaySua = DateTime.Now;
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        // quản lý nhóm người dùng
        [RoleAuthorize(Roles = "0,10=1")]
        public async Task<ActionResult> QuanLyNhomNguoiDung() {
            return View();
        }

        [RoleAuthorize(Roles = "0")]
        public async Task<JsonResult> getQuanLyNhomNguoiDung()
        {
            var model = db.HT_NhomNguoiDung.Where(it => it.Xoa != true && it.IdNhom>0).ToList();
            JsonStatus js = new JsonStatus();
            js.data = model;
            return Json(js, JsonRequestBehavior.AllowGet);
        }

        [RoleAuthorize(Roles = "0,10=4")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> XoaNhomNGuoiDung(List<HT_NhomNguoiDung> listNhom) {
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            JsonStatus js = new JsonStatus();
            js.code = 0;
            js.text = "Thất bại";
            List<int> id = listNhom.Select(it => it.IdNhom).ToList();
            var del = db.HT_NhomNguoiDung.Where(it=> id.Contains(it.IdNhom)).ToList();
            del.All(c => { c.Xoa = true; c.NgayXoa = DateTime.Now; c.NguoiTao = nguoidung.IdNguoiDung; return true; });
            if (db.SaveChanges() > 0)
            {
                js.code = 1;
                js.text = "Thành công";
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [RoleAuthorize(Roles = "0,10=1")]
        public async Task<ActionResult> NhomNguoiDung(int? id, string TenNhom)
        {
            PhanQuyenModel model = new PhanQuyenModel();
            HT_NhomNguoiDung nhomnguoidung = new HT_NhomNguoiDung();
  
            List< HT_NhomQuyen> NhomQuyen = db.HT_NhomQuyen.Where(it=>it.HienThi == true).ToList();
            List<HTQuyen> Quyen = new List<HTQuyen>();

            var QuyenHT = db.HT_Quyen.Where(it => it.HienThi == true).OrderBy(it=> new { it.IdNhomQuyen,it.SapXep}).ToList();
            var CongDoan = (from a in db.HS_CongDoan
                            where a.HienThi == true
                            select new CongDoan { MaCongDoan = a.MaCongDoan, TenCongDoan = a.TenCongDoan, Check = false }).ToList() ;
            var MenuMobile = db.HS_Menu.ToList();
            if (id == null)
            {
                model.CongDoan = CongDoan;
                if (NhomQuyen.Count > 0)
                {

                    foreach (var item in NhomQuyen)
                    {
                        if (item.IdNhomQuyen != 6)
                        {
                            var check = QuyenHT.Where(it => it.IdNhomQuyen == item.IdNhomQuyen).ToList();
                            if (check.Count > 0)
                            {
                                foreach (var item1 in check)
                                {
                                    Quyen.Add(new HTQuyen
                                    {
                                        IdQuyen = item1.IdQuyen,
                                        IdNhomQuyen = item1.IdNhomQuyen,
                                        TenQuyen = item1.TenQuyen,
                                        Xem = false,
                                        ThemMoi = false,
                                        Sua = false,
                                        Xoa = false,
                                        Duyet = false,
                                        Print = false,
                                        Mobile = false
                                    });
                                }

                            }
                        }
                        else if (item.IdNhomQuyen == 6)
                        {
                            foreach (var item1 in MenuMobile)
                            {
                                Quyen.Add(new HTQuyen
                                {
                                    IdQuyen = item1.MenuIndex,
                                    IdNhomQuyen = 6,
                                    TenQuyen = item1.MenuName,
                                    Xem = false,
                                    ThemMoi = false,
                                    Sua = false,
                                    Xoa = false,
                                    Duyet = false,
                                    Print = false,
                                    Mobile = true
                                });
                            }
                        }
                       
                        
                    }
                }
                
                
            }
            else
            {
                nhomnguoidung = db.HT_NhomNguoiDung.FirstOrDefault(it => it.IdNhom == id);

                /// Kiem tra da phan quyền chưa
                var checkPhanQuyen = db.HT_PhanQuyen.Where(it => it.LaNguoiDung == false && it.IdDoiTuong == id).ToList();
                bool Xem, Them, Sua, Xoa, Duyet, Print,Mobile;
                Xem = Them = Sua = Xoa = Duyet = Print = Mobile = false;
                string sQuyen = "",sTemp = "";
                if (NhomQuyen.Count > 0)
                {
                    foreach (var item in NhomQuyen)
                    {
                        if (item.IdNhomQuyen != 6)
                        {
                            var checkQuyen = QuyenHT.Where(it => it.IdNhomQuyen == item.IdNhomQuyen).ToList();
                            if (checkQuyen.Count > 0)
                            {
                                foreach (var item1 in checkQuyen)
                                {
                                    var check = checkPhanQuyen.FirstOrDefault(it => it.IdQuyen == item1.IdQuyen && it.Mobile == false);
                                    if (check != null) // Kiểm tra có phân quyền chưa
                                    {
                                        sTemp = check.IdQuyen.ToString().Trim() + "=";
                                        Xem = Them = Sua = Xoa = Duyet = Print = false;
                                        sQuyen = check.Quyen.ToString().Trim();
                                        if (sQuyen.IndexOf(sTemp + "1") > -1)
                                        {
                                            Xem = true;
                                        }
                                        if (sQuyen.IndexOf(sTemp + "2") > -1)
                                        {
                                            Them = true;
                                        }
                                        if (sQuyen.IndexOf(sTemp + "3") > -1)
                                        {
                                            Sua = true;
                                        }
                                        if (sQuyen.IndexOf(sTemp + "4") > -1)
                                        {
                                            Xoa = true;
                                        }
                                        if (sQuyen.IndexOf(sTemp + "5") > -1)
                                        {
                                            Duyet = true;
                                        }
                                        if (sQuyen.IndexOf(sTemp + "6") > -1)
                                        {
                                            Print = true;
                                        }
                                        Quyen.Add(new HTQuyen
                                        {
                                            IdQuyen = item1.IdQuyen,
                                            IdNhomQuyen = item1.IdNhomQuyen,
                                            TenQuyen = item1.TenQuyen,
                                            Xem = Xem,
                                            ThemMoi = Them,
                                            Sua = Sua,
                                            Xoa = Xoa,
                                            Duyet = Duyet,
                                            Print = Print,
                                            Quyen = sQuyen,
                                            Mobile = false

                                        });
                                    }
                                    else //CHưa phân quyền
                                    {
                                        Quyen.Add(new HTQuyen
                                        {
                                            IdQuyen = item1.IdQuyen,
                                            IdNhomQuyen = item1.IdNhomQuyen,
                                            TenQuyen = item1.TenQuyen,
                                            Xem = false,
                                            ThemMoi = false,
                                            Sua = false,
                                            Xoa = false,
                                            Duyet = false,
                                            Print = false,
                                            Quyen = "",
                                            Mobile = false
                                        });
                                    }
                                }

                            }
                        }
                        else if (item.IdNhomQuyen == 6)
                        {
                            foreach (var item1 in MenuMobile)
                            {
                                var checkQuyenMobile = checkPhanQuyen.Where(it => it.IdQuyen == item1.MenuIndex && it.Mobile == true).ToList();
                                if (checkQuyenMobile.Count > 0)
                                {
                                    Quyen.Add(new HTQuyen
                                    {
                                        IdQuyen = item1.MenuIndex,
                                        IdNhomQuyen = 6,
                                        TenQuyen = item1.MenuName,
                                        Xem = true,
                                        ThemMoi = false,
                                        Sua = false,
                                        Xoa = false,
                                        Duyet = false,
                                        Print = false,
                                        Mobile = true
                                    });
                                }
                                else
                                {
                                    Quyen.Add(new HTQuyen
                                    {
                                        IdQuyen = item1.MenuIndex,
                                        IdNhomQuyen = 6,
                                        TenQuyen = item1.MenuName,
                                        Xem = false,
                                        ThemMoi = false,
                                        Sua = false,
                                        Xoa = false,
                                        Duyet = false,
                                        Print = false,
                                        Mobile = true
                                    });
                                }
                                
                            }
                        }
                        
                    }
                }
                ///check pham vi
                var checkPhamVi = (from a in db.HS_CongDoan 
                                  join b in db.HT_PhamViCongDoan on a.MaCongDoan equals b.MaPhamVi
                                  where b.LaNguoiDung == false
                                        && b.IdDoiTuong == id
                                  select new CongDoan {MaCongDoan = a.MaCongDoan,TenCongDoan = a.TenCongDoan}).ToList();
                if (checkPhamVi.Count > 0)
                {
                    foreach (var item in CongDoan)
                    {

                        var check = checkPhamVi.FirstOrDefault(it => it.MaCongDoan == item.MaCongDoan);
                        if (check != null)
                        {
                            item.Check = true;
                        }
                    }
                }
                model.CongDoan = CongDoan;
            }
            model.NhomNguoiDung = nhomnguoidung;
            model.QuyenSuDung = Quyen;
            model.HTNhomQuyen = NhomQuyen;
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,10=2,10=3")]
        public async Task<JsonResult> AddNhomNguoiDung(int? id, string TenNhom)
        {
            JsonStatus rs = new JsonStatus();
            rs.code = 0;
            rs.text = "Thất bại";
            var nguoidung = Users.GetNguoiDung(User.Identity.Name);
            if (id ==0)
            {
                // Thêm nhóm người dùng
                //Kiểm tra nhom người dùng có tồn tại không

                var checkNhom = db.HT_NhomNguoiDung.FirstOrDefault(it => it.TenNhom.Trim().ToLower() == TenNhom.Trim().ToLower());
                if (checkNhom != null)
                {
                    rs.text = "Tên nhóm đã tồn tại không thể thêm";
                }
                else
                {
                    HT_NhomNguoiDung add = new HT_NhomNguoiDung();
                    add.TenNhom = TenNhom;
                    add.NguoiTao = nguoidung.IdNguoiDung;
                    add.NgayTao = DateTime.Now;
                    db.HT_NhomNguoiDung.Add(add);
                    if (db.SaveChanges() > 0)
                    {
                        rs.code = 1;
                        rs.text = "Thành công";
                        rs.data = db.HT_NhomNguoiDung.FirstOrDefault(it => it.TenNhom == TenNhom && it.NguoiTao == nguoidung.IdNguoiDung);
                    }
                }
            }
            else {
                //Sửa nhóm người dùng
                var edit = db.HT_NhomNguoiDung.FirstOrDefault(it => it.IdNhom == id);
                var checkNhom = db.HT_NhomNguoiDung.FirstOrDefault(it => it.TenNhom.Trim().ToLower() == TenNhom.Trim().ToLower() && it.IdNhom != id);
                if (checkNhom != null)
                {
                    rs.text = "Tên nhóm đã tồn tại không thể sửa";
                    
                }
                else
                {
                    edit.TenNhom = TenNhom;
                    edit.NgaySua = DateTime.Now;
                    edit.NguoiSua = nguoidung.IdNguoiDung;
                    if (db.SaveChanges() > 0)
                    {
                        rs.code = 1;
                        rs.text = "Thành công";
                        rs.data = edit;
                    }
                }
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "0,10=2,10=3")]
        public async Task<JsonResult> LuuPhanQuyenNhomNguoiDung(List<HT_PhanQuyen> list,List<CongDoan> CongDoan)
        {
            JsonStatus rs = new JsonStatus();
            try
            {
                using (var tran = new TransactionScope())
                {
                    var nguoidung = Users.GetNguoiDung(User.Identity.Name);
                    int idDoiTuong = list[0].IdDoiTuong;
                    var data = db.HT_PhanQuyen.Where(it => it.LaNguoiDung == false && it.IdDoiTuong == idDoiTuong).ToList();
                    if (data.Count > 0)
                    {
                        db.HT_PhanQuyen.RemoveRange(data);
                        list.All(c => { c.NguoiTao = nguoidung.IdNguoiDung; c.NgayTao = DateTime.Now; return true; });
                       // List<HT_PhanQuyen> add = list.Cast<HT_PhanQuyen>().ToList();
                        db.HT_PhanQuyen.AddRange(list);
                    }
                    else
                    {
                        list.All(c => { c.NguoiTao = nguoidung.IdNguoiDung; c.NgayTao = DateTime.Now; return true; });
                        //List<HT_PhanQuyen> add = list.Cast<HT_PhanQuyen>().ToList();
                        db.HT_PhanQuyen.AddRange(list);
                    }
                    var checkPhamVi = db.HT_PhamViCongDoan.Where(it => it.IdDoiTuong == idDoiTuong && it.LaNguoiDung == false).ToList();
                    if (checkPhamVi !=null && checkPhamVi.Count > 0)
                        db.HT_PhamViCongDoan.RemoveRange(checkPhamVi);
                    if (CongDoan != null && CongDoan.Count > 0)
                    {
                        List<HT_PhamViCongDoan> listCongDoan = new List<HT_PhamViCongDoan>();
                        foreach (var item in CongDoan)
                        {
                            HT_PhamViCongDoan add = new HT_PhamViCongDoan();
                            add.IdDoiTuong = idDoiTuong;
                            add.LaNguoiDung = false;
                            add.MaPhamVi = item.MaCongDoan;
                            listCongDoan.Add(add);
                        }
                        db.HT_PhamViCongDoan.AddRange(listCongDoan);

                    }

                    if (db.SaveChanges() > 0)
                    {
                        rs.code = 1;
                        rs.text = "Thành công";
                        rs.data = db.HT_NhomNguoiDung.FirstOrDefault(it => it.IdNhom == idDoiTuong);
                        tran.Complete();
                        tran.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                rs.code = 0;
                rs.text = ex.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
    }
}