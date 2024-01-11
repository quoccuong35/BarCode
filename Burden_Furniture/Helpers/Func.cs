using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DevExpress.Spreadsheet.Export;
using DevExpress.Spreadsheet;
using System.Runtime.Caching;

namespace Helpers
{
    public class Func
    {
        public DataTable GetDataFromExcel(HttpPostedFileBase file, int SheetIndex, string Begin, string End, int StartColumn)
        {
            try
            {
                Workbook workbook = new Workbook();
                workbook.LoadDocument(file.InputStream);
                Worksheet xlSheet = workbook.Worksheets[SheetIndex];
                CellRange rangeTotal = xlSheet.GetUsedRange();
                string r = string.Format("{0}{1}:{2}{3}", Begin, StartColumn, End, (rangeTotal.BottomRowIndex + 1));
                CellRange range = xlSheet.Range[r];
                DataTable dataTable = xlSheet.CreateDataTable(range, true);
                DataTableExporter exporter = xlSheet.CreateDataTableExporter(range, dataTable, true);
                exporter.Export();
                return dataTable;
            }
            catch
            {
                return new DataTable();
            }
        }
        public DataTable GetDataFromExcelFile(string file, int SheetIndex, string Begin, string End, int StartColumn)
        {
            try
            {
                Workbook workbook = new Workbook();
                workbook.LoadDocument(file);
                Worksheet xlSheet = workbook.Worksheets[SheetIndex];
                CellRange rangeTotal = xlSheet.GetUsedRange();
                string r = string.Format("{0}{1}:{2}{3}", Begin, StartColumn, End, (rangeTotal.BottomRowIndex + 1));
                CellRange  range = xlSheet[r];
                DataTable dataTable = xlSheet.CreateDataTable(range, true);
                DataTableExporter exporter = xlSheet.CreateDataTableExporter(range, dataTable, true);
                exporter.CellValueConversionError += exporter_CellValueConversionError;
                exporter.Export();
                return dataTable;
            }
            catch (Exception ex)
            {
                string Loi = ex.ToString();
                return new DataTable();
            }
        }
        void exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
        {
            Console.WriteLine("Error in cell " + e.Cell.GetReferenceA1());
            e.DataTableValue = null;
            e.Action = DataTableExporterAction.Continue;
        }
        public string GetFilter(List<GridFilter> f)
        {
            string sWhere = "";
            int d = 0;
            foreach (var item in f)
            {
                if (d > 0)
                    sWhere += " AND ";
                sWhere += GetCompare(item);
                d++;
            }
            return sWhere;
        }
       
        public string GetFilterOR(List<GridFilter> f)
        {
            string sWhere = "";
            int d = 0;
            foreach (var item in f)
            {
                if (d > 0)
                    sWhere += " OR ";
                sWhere += GetCompare(item);
                d++;
            }
            return "(" + sWhere + ")";
        }
   
        public string GetCompare(GridFilter f)
        {
            string sWhere = "";
            string value = f.Value.Replace("'", "'+char(39)+'");
            if (injectionCheck(value))
            {
                sWhere = " 1=1 ";
            }
            else
            if (f.Compare == "contains")
            {
                sWhere = f.Column + " like N'%" + value + "%' ";
            }
            else if (f.Compare == "notcontains")
            {
                sWhere = f.Column + " not like N'%" + value + "%' ";
            }
            else if (f.Compare == "startswith")
            {
                sWhere = f.Column + " like N'%" + value + "' ";
            }
            else if (f.Compare == "endswith")
            {
                sWhere = f.Column + " like N'%" + value + "' ";
            }
            else if (f.Compare == "=" || f.Compare == "<>" || f.Compare == ">" || f.Compare == "<" || f.Compare == ">=" || f.Compare == "<=")
            {
                sWhere = f.Column + " " + f.Compare + " N'" + value + "' ";
            }
            else if (f.Compare.ToLower().Trim() == "in" || f.Compare.ToLower().Trim() == "not in")
            {
                string[] temp = value.Split(',');
                value = "";
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i].Trim() == null || temp[i].Trim() == "")
                        break;
                    value += "N'" + temp[i].ToString().Trim() + "',";
                }
                if (value.Length > 0)
                {
                    value = value.Substring(0, value.Length - 1);
                    sWhere = f.Column + " " + f.Compare + " (" + value + ") ";
                }
                else
                {
                    sWhere = " 1 = 1 ";
                }
            }
            else
            {
                sWhere = " 1=1 ";
            }
            return sWhere;
        }
        public bool injectionCheck(string queryString)
        {
            var badWords = new[] {
            "EXEC", "EXECUTE", ";", "-", "*", "--", "@",
            "UNION", "DROP","DELETE", "UPDATE", "INSERT",
            "MASTER", "TABLE", "XP_CMDSHELL", "CREATE",
             "SYSCOLUMNS", "SYSOBJECTS"
        };
            string pattern = "(?<!\\w)(" + Regex.Escape(badWords[0]);

            foreach (var key in badWords.Skip(1))
            {
                pattern += "|" + Regex.Escape(key);
            }

            pattern += ")(?!\\w)";

            dynamic _tmpCount = Regex.Matches(queryString, pattern, RegexOptions.IgnoreCase).Count;

            if (_tmpCount >= 1)
                return true;
            else
                return false;
        }
        public string SetQRCode1ByKey(string id)
        {
            try
            {
                string LogName = Guid.NewGuid().ToString();
                MemoryCache.Default.Add(LogName, id, DateTime.Now.AddMinutes(30));
                return LogName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetQRCode1ByKey(string id)
        {
            try
            {
                return MemoryCache.Default.Get(id).ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
