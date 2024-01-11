using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using System.Web.Security;
using Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WMS.DB;
using WMS.Models;

namespace WMS.Controllers
{
    [RoleAuthorize]
    public class AccountController : Controller
    {
        WMSEntities db = new WMSEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult GetLogin(string Username, string Password)
        {
            string[] rs = { "1", Url.Action("Index", "Home") };
            try
            {
                Password = Helpers.Encode_Decode.Encrypt(Password);
                var nguoidung = db.PR_ThongTinNguoiDung(Username).FirstOrDefault();
                if (nguoidung != null && nguoidung.MatKhau == Password && nguoidung.HoatDong == true && nguoidung.Xoa != true)
                {
                    FormsAuthentication.SetAuthCookie(Username, true);
                    var ng = new NguoiDung();
                    ng.IdNguoiDung = nguoidung.IdNguoiDung;
                    ng.IdNhom = nguoidung.IdNhom;
                    ng.TaiKhoan = nguoidung.TaiKhoan;
                    ng.HoTen = nguoidung.HoTen;
                    ng.TenNhom = nguoidung.TenNhom;
                    Users.SetNguoiDung(ng);
                }
                else
                {
                    rs[0] = "0";
                    rs[1] = "Thông tin đăng nhập không đúng.";
                }
            }
            catch (Exception ex)
            {
                rs[0] = "0";
                rs[1] = "Đã có lỗi xảy ra, xin vui lòng thử lại.";
            }
            return Json(rs);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }


    }
}