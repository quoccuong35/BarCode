using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helpers;
using WMS.DB;
namespace WMS.Models
{
    public class Users
    {
        
        public static void SetNguoiDung(NguoiDung nguoiDung)
        {
            ThongTinNguoiDung thongtin = new ThongTinNguoiDung();
            thongtin.SetNguoiDung(nguoiDung);
        }
        public static NguoiDung GetNguoiDung(string Username)
        {
            ThongTinNguoiDung thongtin = new ThongTinNguoiDung();
            var ng = thongtin.GetNguoiDung(Username);
            if (ng == null)
            {
                WMSEntities db = new WMSEntities();
                var nguoidung = db.PR_ThongTinNguoiDung(Username).FirstOrDefault();
                ng = new NguoiDung();
                ng.IdNguoiDung = nguoidung.IdNguoiDung;
                ng.IdNhom = nguoidung.IdNhom;
                ng.TaiKhoan = nguoidung.TaiKhoan;
                ng.HoTen = nguoidung.HoTen;
                ng.TenNhom = nguoidung.TenNhom;
            }
            return ng;
        }
    }
}