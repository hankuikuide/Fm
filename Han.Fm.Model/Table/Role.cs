/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/30 18:23:44
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
    /// TODO: Update summary.
    /// </summary>
    [Table("FM_ROLE")]
    public class Role
    {
        #region Fields
        /// <summary>
        /// 主键
        /// </summary>
        [SequenceId("ID", "SEQ_FM_ROLE.NEXTVAL")]
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Column("ROLE_NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }

        /// <summary>
        /// 状态（1：正常0：禁用）
        /// </summary>
        [Column("STATE")]
        public int State { get; set; }

        /// <summary>
        /// 0其他角色1初审人员2复审人员3终审人员4稽查人员
        /// </summary>
        [Column("SYS_APP_KEY")]
        public string SysAppKey { get; set; }

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Methods and Operators

        #endregion
    }
}
