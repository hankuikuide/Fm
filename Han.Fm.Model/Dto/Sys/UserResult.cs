/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/24 9:12:04
 * ***********************************************/

namespace Han.Fm.Model.Dto.Sys
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
    [Dto]
    public class UserResult
    {
        #region Fields
        
        [Alias("ID")]
        public string Id { get; set; }

        [Alias("Name")]
        public string Name { get; set; }

        [Alias("USER_CODE")]
        public string Code { get; set; }

        [Alias("USER_PASSWORD")]
        public string Password { get; set; }

        [Alias("USER_TELEPHONE")]
        public string Telephone { get; set; }

        [Alias("USER_EMAIL")]
        public string Email { get; set; }

        [Alias("SORT")]
        public decimal Sort { get; set; }

        [Alias("REMARK")]
        public string Remark { get; set; }

        [Alias("ErrorTimes")]
        public decimal ErrorTimes { get; set; }
        
        public DateTime? LastUpdateTime { get; set; }

        public DateTime LastPasswordUpdate { get; set; }

        public int EnableFlag { get; set; }

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Methods and Operators

        #endregion
    }
}
