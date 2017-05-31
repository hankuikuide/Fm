/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/24 9:27:15
 * ***********************************************/

namespace Han.Fm.Dal.Sys
{
    using DbLight.MySQl;
    using DbLight.Oracle;
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
    public class UserDao:MySqlSingleTableDao<User>
    {
        public UserDao():base(DatabaseFactory.CreateQuerySession())
        {
        }

        public List<UserResult> GetUsers()
        {
            var sql = @"select su.id,
                           user_code      Code,
                           user_name      Name,
                           su.latest_password_update LastPasswordUpdate,
                           LATEST_UPDATE_TIME LastUpdateTime,
                           su.enable_flag EnableFlag
                      from fm_user su
                     where su.issys_flag != '1'
                     order by su.sort";

            return QuerySession.ExecuteSqlString<UserResult>(sql).ToList();
        }
    }
}
