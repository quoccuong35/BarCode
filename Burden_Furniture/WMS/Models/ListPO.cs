using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;
using WMS.DB;
namespace WMS.Models
{
    public class ListPO
    {
        public List<BDPO> listPO {get;set;}
        public List<BD_POChiTiet> listPOChiTiet { get; set; }
    }
    public class BDPO : BD_Po
    {
        public String NgayHopDong { get; set; }
    }
}