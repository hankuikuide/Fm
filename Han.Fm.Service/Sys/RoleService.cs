/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/30 18:41:18
 * ***********************************************/

namespace Han.Fm.Service.Sys
{
    using Dal.Sys;
    using Model.BaseDto;
    using Model.Dto.Sys;
    using Model.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RoleService
    {
        private readonly RoleDao roleDao = new RoleDao();

        public Response<List<RoleResult>> GetRoles()
        {
            var result = new Response<List<RoleResult>>();

            result.Result = roleDao.GetRoles();

            return result;
        }

        public Response<bool> CreateRole(RoleResult role)
        {
            var result = new Response<bool>();

            List<Role> roles = new List<Role>();
            roles.Add(new Role
            {
                Name = role.RoleName,
                Remark = role.Remark,
                State = 1
            });

            roleDao.BatchInsert(roles, r => r.Name, r => r.Remark, r => r.State);

            return result;
        }
        public Response<bool> UpdateRole(RoleResult role)
        {
            var result = new Response<bool>();

            List<Role> roles = new List<Role>();
            roles.Add(new Role
            {
                Name = role.RoleName,
                Remark = role.Remark,
                State = 1
            });

            roleDao.BatchUpdate(roles, "id=?3", r => r.Name, r => r.Remark, r => r.State, r => r.Id);

            return result;
        }
        public Response<bool> RemoveRole(string roleId)
        {
            var result = new Response<bool>();

            List<Role> roles = new List<Role>();
            roles.Add(new Role
            {
                Id = roleId 
            });

            roleDao.BatchDelete(roles, "id=?0", r => r.Id);

            return result;
        }
    }
}
