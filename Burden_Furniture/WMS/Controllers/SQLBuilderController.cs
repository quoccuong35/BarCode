using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Libs;
using System.Web.Mvc;
using WMS.Models;
using WMS.DB;
namespace WMS.Controllers
{
    [RoleAuthorize(Roles = "0")]
    public class SQLBuilderController : Controller
    {
        // GET: SQLBuilder
        public ActionResult Index()
        {
            return View();
        }
        WMSEntities db = new WMSEntities();
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetProc()
        {
            return Json(await db.HT_SQL.Select(m => new { m.Id, m.Name }).ToListAsync(), JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetSQL(string id)
        {
            return Json(await db.HT_SQL.Where(m => m.Id == id).FirstOrDefaultAsync(), JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<JsonResult> Update(string id,string sql)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var s = await db.HT_SQL.FirstOrDefaultAsync(m => m.Id == id);
                if (s != null)
                {
                    s.SQL = sql;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var sq = new HT_SQL();
                    sq.Name = id;
                    sq.SQL = sql;
                    db.HT_SQL.Add(sq);
                    await db.SaveChangesAsync();
                  
                }
                st.code = 1;
                st.text = "Success";
            }
            catch(Exception ex)
            {
                st.code = 0;
                st.text = ex.Message;
            }
            return Json(st);
        }
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<JsonResult> ThemMoiThamSo(string Id, string Name)
        {
            JsonStatus st = new JsonStatus();
            try
            {
                var s = await db.HT_SQL.FirstOrDefaultAsync(m => m.Id == Id);
                if (s != null)
                {
                    st.code = 0;
                    st.text = "Tham số đã tồn tại";
                }
                else
                {
                    var sq = new HT_SQL();
                    sq.Name = Name;
                    sq.Id = Id;
                    db.HT_SQL.Add(sq);
                    await db.SaveChangesAsync();
                    st.code = 1;
                    st.text = "Success";
                }
             
            }
            catch (Exception ex)
            {
                st.code = 0;
                st.text = ex.Message;
            }
            return Json(st);
        }
    }
}