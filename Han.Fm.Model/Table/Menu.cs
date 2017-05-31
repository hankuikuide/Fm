/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/20 15:29:09
 * ***********************************************/

namespace Han.Fm.Model.Table
{
    using DbLight.TableMetadata;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    [Table("FM_MENU")]
    public class Menu
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SequenceId("ID", "seq_fm_menu.nextval")]
        public decimal Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Column("MENU_NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        [Column("MENU_TYPE")]
        public string MenuType { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Column("SORT")]
        public decimal Sort { get; set; }

        /// <summary>
        /// 父菜单
        /// </summary>
        [Column("PARENT_ID")]
        public decimal ParentId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column("STATE")]
        public string State { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Column("VIEW_PARAMS")]
        public string ViewParams { get; set; }

        /// <summary>
        /// 菜单权限验证码
        /// </summary>
        [Column("VALIDATE_CODE")]
        public string ValidateCode { get; set; }

        /// <summary>
        /// 菜单事件
        /// </summary>
        [Column("HANDLER")]
        public string Handler { get; set; }

        /// <summary>
        /// 菜单视图
        /// </summary>
        [Column("MENU_VIEW")]
        public string MenuView { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [Column("ICON")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否为功能
        /// </summary>
        [Column("OPERATION_TYPE")]
        public string OperationType { get; set; }

        /// <summary>
        /// 管理员菜单标识
        /// </summary>
        [Column("SYS_APP_KEY")]
        public string SysAppKey { get; set; }
    }
}
