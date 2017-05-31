/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/30 18:22:29
 * ***********************************************/

namespace Han.Fm.Dal.Sys
{
    using DbLight.MySQl;
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
    public class RoleDao:MySqlSingleTableDao<Role>
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        public RoleDao():base(DatabaseFactory.CreateQuerySession())
        {

        }

        #endregion

        #region Public Methods and Operators

        public List<RoleResult> GetRoles()
        {
            var sql = @"select role_name rolename, remark,state from fm_role";

            return QuerySession.ExecuteSqlString<RoleResult>(sql).ToList();
        }

        #endregion
    }
}
