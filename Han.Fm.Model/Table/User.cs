/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/24 9:25:32
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
    /// 用户
    /// </summary>
    [Table("FM_USER")]
    public class User
    {
        /// <summary>
        /// 主键,标识列,序列 就这样
        /// </summary>      
        [SequenceId("ID", "SEQ_FM_USER.NEXTVAL")]
        public string Id { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [Column("USER_CODE")]
        public string Code { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Column("USER_NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Column("USER_PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Column("USER_TELEPHONE")]
        public string Telephone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Column("USER_EMAIL")]
        public string Email { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Column("SORT")]
        public decimal? Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }

        /// <summary>
        /// 状态（1：正常0：禁用）
        /// </summary>
        [Column("ENABLE_FLAG")]
        public string EnableFlag { get; set; }

        /// <summary>
        /// 是否内置角色（1：是0：否）
        /// </summary>
        [Column("ISSYS_FLAG")]
        public string IsSysFlag { get; set; } = "0";

        /// <summary>
        /// 用户类型
        /// </summary>
        [Column("USER_TYPE")]
        public string UserType { get; set; }

        /// <summary>
        /// 用户机构
        /// </summary>
        [Column("OrgId")]
        public string OrgId { get; set; }

        /// <summary>
        /// 用户登录密钥
        /// </summary>
        [Column("Login_Key")]
        public string LoginKey { get; set; }

        /// <summary>
        /// 最近密码修改时间
        /// </summary>
        [Column("LATEST_PASSWORD_UPDATE")]
        public DateTime? LatestPassUptime { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Column("USER_WORKNO")]
        public string WorkNo { get; set; }

        /// <summary>
        /// 登录错误连续次数
        /// </summary>
        [Column("ERROR_TIMES")]
        public decimal? ErrorTimes { get; set; }

        /// <summary>
        /// 最后一次错误登录时间
        /// </summary>
        [Column("LAST_ERROR_TIME")]
        public DateTime? LastErrorTime { get; set; }
    }
}
