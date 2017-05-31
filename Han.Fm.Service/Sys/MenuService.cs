/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/20 15:42:18
 * ***********************************************/

namespace Han.Fm.Service.Sys
{
    using Dal.Sys;
    using Model.BaseDto;
    using Model.Dto.Sys;
    using Model.Table;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MenuService
    {
        private readonly MenuDao menuDao = new MenuDao();
        public Response<List<MenuResult>> GetMenus()
        {
            var res = new Response<List<MenuResult>>();

            res.Result = menuDao.GetMenus();

            return res;
        }

        public Response<bool> SaveMenu(MenuResult node)
        {
            var result = new Response<bool>();

            Menu menu = new Menu()
            {
                Id = node.Id,
                Name = node.Name,
                Handler = node.Handler,
                Icon = node.Icon ?? "",
                Remark = node.Remark ?? ""
            };

            menuDao.BatchUpdate(new List<Menu> { menu }, "id=?4", n => n.Name, n => n.Handler, n => n.Icon, n => n.Remark, n => n.Id);

            result.Result = true;

            return result;

        }

        public Response<bool> RemoveMenu(decimal menuId)
        {
            var result = new Response<bool>();
            List<Menu> menus = new List<Menu>();
            menus.Add(new Menu { Id = menuId });

            menuDao.BatchDelete(menus, "id=?0", u => u.Id);

            return result;

        }
    }
}
