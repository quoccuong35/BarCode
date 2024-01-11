using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.DB;

namespace WMS.Models
{
    public class PhanQuyenModel
    {
        public HT_NhomNguoiDung NhomNguoiDung { get; set; }
        public List<HT_NhomQuyen> HTNhomQuyen { get; set; }
        public List<HTQuyen> QuyenSuDung { get; set; }
        public List<CongDoan> CongDoan { get; set; }
    }
    public class HTQuyen : HT_Quyen
    {
        public bool Xem { get; set; } //1
        public bool ThemMoi { get; set; }//2
        public bool Sua { get; set; }//3
        public bool Xoa { get; set; }//4
        public bool Duyet { get; set; }//5
        public bool Print { get; set; }//6
        public string Quyen { get; set; }
        public bool Mobile { get; set; }
    }
    public class CongDoan:HS_CongDoan {
        public bool Check { get; set; }
    }
}