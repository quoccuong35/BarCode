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
    
    public partial class BD_PhieuNhapHangChiTiet
    {
        public long IDPhieuNhapHangChiTiet { get; set; }
        public long IdPhieuNhapHang { get; set; }
        public long IDPOChiTiet { get; set; }
        public string SoPhieu { get; set; }
        public string SoPO { get; set; }
        public string SoLichGiaoHang { get; set; }
        public string MaSanPham { get; set; }
        public string MaChiTietSanPham { get; set; }
        public string TenChiTietSanPham { get; set; }
        public string MaKe { get; set; }
        public string QRCode { get; set; }
        public int SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public string GhiChu { get; set; }
        public Nullable<int> NguoiDungTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> NguoiDungSua { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public Nullable<bool> XuatKho { get; set; }
        public string SoEPI { get; set; }
        public Nullable<System.DateTime> NgayXuatHang { get; set; }
        public Nullable<int> IdKe { get; set; }
        public Nullable<int> NguoiXuat { get; set; }
        public Nullable<int> NumberOf { get; set; }
    }
}