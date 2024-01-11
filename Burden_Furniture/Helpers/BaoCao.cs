using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Helpers
{
    public class BaoCao
    {
        public string  reportFileFullName()
        {
          return HttpContext.Current.Request.Cookies["filename"].Value.ToString();
        }
        public string reportName()
        {
            string file = reportFileFullName();
            string[] filename = Regex.Split(file, "__");
            return filename[filename.Length - 1];
        }
        public string fileName(string reportname,string filename)
        {
            string _filename = HttpContext.Current.Server.MapPath("~/Content/upload/export/") + "__" + reportname + "_" + filename + ".xlsx";
            var httpCookie = HttpContext.Current.Request.Cookies["filename"];
            if (httpCookie != null)
            {
                var cookie = HttpContext.Current.Response.Cookies["filename"];
                if (cookie != null)
                    cookie.Value = _filename;
            }
            else
            {
                HttpCookie f = new HttpCookie("filename");
                f.Value = _filename;
                f.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.Cookies.Add(f);
            }
            return _filename;
        }
        public byte[] fileReport()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                string file = reportName();
                string[] filename = Regex.Split(file, "__");
                DevExpress.Web.Mvc.SpreadsheetExtension.GetCurrentDocument("ViewReport").SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                return stream.ToArray();
            }
        }
        public void clearReport()
        {
            try
            {
                string p = HttpContext.Current.Server.MapPath("~/Content/upload/export/");
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);
                string[] sFile = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Content/upload/export/"), "*.xlsx", SearchOption.AllDirectories);
                if (sFile.Length > 0)
                {
                    for (int i = 0; i < sFile.Length; i++)
                    {
                        FileInfo f = new FileInfo(sFile[i]);
                        DateTime dtServer = f.LastWriteTime;
                        DateTime n = DateTime.Now;
                        if (dtServer.AddMinutes(30) < n)
                        {
                            System.IO.File.Delete(f.FullName);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
