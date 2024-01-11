using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
namespace Helpers
{
    public class ThongTinNguoiDung
    {
        public void SetNguoiDung(NguoiDung nguoiDung)
        {
            try
            {
                MemoryCache.Default.Add(nguoiDung.TaiKhoan, nguoiDung, DateTime.Now.AddDays(1));
            }
            catch
            {

            }
        }
        public NguoiDung GetNguoiDung(string Username)
        {
            try
            {
                return (NguoiDung)MemoryCache.Default.Get(Username);
            }
            catch
            {
                return null;
            }
        }
    }
    public  class NguoiDung
    {
        public int IdNguoiDung { get; set; }
        public string TaiKhoan { get; set; }
        public string HoTen { get; set; }
        public Nullable<int> IdNhom { get; set; }
        public string TenNhom { get; set; }
      
    }
    public class GridFilter
    {
        public string Column { get; set; }
        public string Compare { get; set; }
        public string Value { get; set; }
    }

}
