//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class HT_NguoiDung
    {
        public int IdNguoiDung { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public Nullable<bool> HoatDong { get; set; }
        public Nullable<int> IdNhom { get; set; }
        public Nullable<bool> Xoa { get; set; }
        public Nullable<int> NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> NguoiSua { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public Nullable<int> NguoiXoa { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
    }
}
