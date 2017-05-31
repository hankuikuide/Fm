/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/20 15:34:59
 * ***********************************************/

namespace Han.Fm.Model.Dto.Sys
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 新版菜单表
    /// </summary>
    public class MenuResult
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单对应界面
        /// </summary>
        public string MenuView { get; set; }


        /// <summary>
        /// 父级菜单编码
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 菜单视图对应参数
        /// </summary>
        public string ViewParams { get; set; }

        /// <summary>
        /// 点击该菜单触发的事件
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 系统标识
        /// </summary>
        public string SysAppKey { get; set; }

        /// <summary>
        /// 菜单权限码
        /// </summary>
        public string ValidateCode { get; set; }

        /// <summary>
        /// 菜单操作类型 1:目录2:菜单3:功能
        /// </summary>
        public string OperationType { get; set; }

        public string MenuType { get; set; }
        public string Remark { get; set; } = "";
        public decimal Sort { get; set; }
        public string State { get; set; }
    }
}
