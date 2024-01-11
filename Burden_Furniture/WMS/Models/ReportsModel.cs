using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.XtraReports.UI;
namespace WMS.Models
{
    public class ReportsModel
    {
        public string ReportName { get; set; }
        public XtraReport Report { get; set; }
    }
}