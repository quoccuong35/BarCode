using Antlr.Runtime.Misc;
using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WMS.DB;
namespace WMS.Models
{
    public class GridData
    {
        WMSEntities db = new WMSEntities();
        public DataTable GetDataTable(string sql)
        {
            try
            {
                System.Web.Libs.Common c = new System.Web.Libs.Common();
                string con = c.GetConnectString(System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString);
                return c.DT_DataTable(sql, con);
            }
            catch
            {
                return null;
            }
        }
        public string GetGridData(string sql)
        {
            try
            {
                System.Web.Libs.Common c = new System.Web.Libs.Common();
                string con = c.GetConnectString(System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString);
                return JsonConvert.SerializeObject(c.DT_DataTable(sql, con), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch
            {
                return null;
            }
        }
        public string GetGridData(string key, int Page, int PageSize,string Filter, int LoadAll)
        {
            try
            {
                Func func = new Func();
                string sWhere = "";
                int NguoiDung = Users.GetNguoiDung(HttpContext.Current.User.Identity.Name).IdNguoiDung;
                if (!string.IsNullOrEmpty(Filter))
                {
                    sWhere = func.GetFilter(JsonConvert.DeserializeObject<List<GridFilter>>(Filter));
                }
                System.Web.Libs.Common c = new System.Web.Libs.Common();
                string con = c.GetConnectString(System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString);
                string sSQL = GetQuery(key);
                sSQL = sSQL.Replace("@PageNumber", Page.ToString()).Replace("@PageSize", PageSize.ToString()).Replace("@NguoiDung", NguoiDung.ToString()).Replace("@LoadAll", LoadAll.ToString());
                if (!string.IsNullOrEmpty(sWhere))
                    sSQL = sSQL.Replace("1 = 1", sWhere);
                return JsonConvert.SerializeObject(c.DT_DataTable(sSQL, con), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch
            {
                return null;
            }
        }
        public string GetGridDataWithWhere(string key, int Page, int PageSize, string Filter, int LoadAll,string sWhere)
        {
            try
            {
                Func func = new Func();
              
                int NguoiDung = Users.GetNguoiDung(HttpContext.Current.User.Identity.Name).IdNguoiDung;
              
                System.Web.Libs.Common c = new System.Web.Libs.Common();
                string con = c.GetConnectString(System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString);
                string sSQL = GetQuery(key);
                sSQL = sSQL.Replace("@PageNumber", Page.ToString()).Replace("@PageSize", PageSize.ToString()).Replace("1 = 1", sWhere).Replace("@NguoiDung", NguoiDung.ToString()).Replace("@LoadAll", LoadAll.ToString());
                return JsonConvert.SerializeObject(c.DT_DataTable(sSQL, con), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch
            {
                return null;
            }
        }
        public string GetQuery(string key)
        {
            return db.HT_SQL.FirstOrDefault(m => m.Id == key).SQL;
        }
    }
}