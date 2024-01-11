using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;
using WMS.DB;
namespace WMS.Models
{
    public class PO
    {
        public BD_Po ThongTinPO { get; set; }
        public List<BD_POChiTiet> POChiTiet { get; set; }
    }
   
}