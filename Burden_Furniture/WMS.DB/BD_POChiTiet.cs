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
    
    public partial class BD_POChiTiet
    {
        public long ID { get; set; }
        public long IDPO { get; set; }
        public string SoPO { get; set; }
        public string MaSanPham { get; set; }
        public string MaChiTietSanPham { get; set; }
        public string SoLuongTrongMoiBo { get; set; }
        public string SoLichGiaoHang { get; set; }
        public string TenChiTietSanPham { get; set; }
        public string TenQuanLyKhoCIRS { get; set; }
        public string DonViTinh { get; set; }
        public Nullable<int> SoLuongPO { get; set; }
        public Nullable<int> SoLuongNhap { get; set; }
        public Nullable<int> NguoiDungTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> NguoiDungSua { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public Nullable<bool> Del { get; set; }
    }
}