using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WMS.DB;
namespace WMS.Models
{
    public class CustomRoleProvider : RoleProvider
    {
        public CustomRoleProvider() { }

        public override string[] GetRolesForUser(string Usertname)
        {
            try
            {
                WMSEntities db = new WMSEntities();
                var list_quyen = db.Database.SqlQuery<string>(string.Format("DECLARE @Id int=(SELECT TOP 1 IdNhom FROM dbo.HT_NguoiDung WHERE TaiKhoan='{0}') IF @Id = 0 OR @Id = 1 BEGIN SELECT   CONVERT(NVARCHAR(150),'0') Quyen END  ELSE BEGIN SELECT Quyen FROM dbo.HT_NguoiDung INNER JOIN dbo.HT_PhanQuyen ON (IdNguoiDung=IdDoiTuong AND LaNguoiDung=1) OR (IdNhom=IdDoiTuong AND LaNguoiDung=0) WHERE TaiKhoan='{0}' AND  Quyen IS NOT NULL AND ISNULL(Mobile,0) = 0 END", Usertname)).ToList();
                string q = string.Join("|", list_quyen);
                return q.Split('|');

            }
            catch
            {
                return new string[] { "" };
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}