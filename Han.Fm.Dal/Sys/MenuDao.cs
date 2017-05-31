/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/20 15:28:30
 * ***********************************************/

namespace Han.Fm.Dal.Sys
{
    using Han.DbLight.Oracle;
    using Han.Fm.Model.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DbLight;
    using Model.Dto.Sys;
    using Han.DbLight.MySQl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MenuDao : MySqlSingleTableDao<Menu>
    {
        public MenuDao():base(DatabaseFactory.CreateQuerySession())
        {
        }

        public List<MenuResult> GetMenus()
        {
            var sql = CreateSqlBuilder(new StringBuilder(@"
                        select distinct sm.id,
                        menu_name name,
                        sm.parent_id parentid,
                        sm.menu_view menuview,
                        sm.menu_type menutype,
                        sm.state state,
                        sm.view_params viewparams,
                        sm.operation_type operationtype,
                        sm.handler,
                        sm.icon,
                        sm.sort,
                        sm.sys_app_key sysappkey,
                        sm.remark remark,
                        sm.validate_code validatecode
                      from fm_menu sm
/*                      left join fm_area_menu fam
                        on fam.menu_id = sm.id*/
                      left join fm_role_menu rm
                        on sm.id = rm.menu_id
                      left join (select ur.user_id, ur.role_id
                                   from fm_user_role ur
                                  inner join fm_role fr
                                     on ur.role_id = fr.id
                                    and fr.state = '1') ur
                        on rm.role_id = ur.role_id
                     where sm.state = '1'
                      -- and (ur.user_id = '2361' or operation_type = '1' or
                        --   (operation_type = '2' and menu_type = '3'))
                        and sys_app_key in ('3601DE249ADF63D4E0531D82750A85F3')
                       and Operation_type in ('1', '2')
                       and (operation_type <> '2' or menu_type in ('1', '2', '3'))
                     order by sort asc"));

            //sql.AppendInWhereHasValue(() => appkey, " and sys_app_key in ({0}) ");
            //sql.AppendInWhereHasValue(() => operationTypes, " and Operation_type in({0})");
            //sql.AppendInWhereHasValue(() => menuTypes, " and (operation_type<>'2' or menu_type in({0}))");
            //sql.Append(" order by sort asc");
            return QuerySession.ExecuteSqlString<MenuResult>(sql.ToSql(), sql.DbParams).ToList();
        }

    }
}
