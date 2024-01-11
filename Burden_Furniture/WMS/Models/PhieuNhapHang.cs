using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.DB;
namespace WMS.Models
{
    public class PhieuNhapHang
    {
        public BD_PhieuNhapHang PNH { get; set; }
        public List<PhieuNhapHangChiTiet> PNHCT { get; set; }
    }
    public class PhieuNhapHangChiTiet
    {
        //public long IdPhieuNhapHang { get; set; }
        //public long IDPOChiTiet { get; set; }
        //public string SoPhieu { get; set; }
        public string SoPO { get; set; }
        public string SoLichGiaoHang { get; set; }
        public string MaSanPham { get; set; }
        public string MaChiTietSanPham { get; set; }
        public string TenChiTietSanPham { get; set; }
        public int SoLuong { get; set; }
        public string DonViTinh { get; set; }
    }
}