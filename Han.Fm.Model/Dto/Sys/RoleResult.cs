/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/30 18:35:45
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
    [Dto()]
    public class RoleResult
    {
        #region Fields

        [Alias("ROLE_NAME")]
        public string RoleName { get; set; }

        [Alias("REMARK")]
        public string Remark { get; set; }

        [Alias("STATE")]
        public int State { get; set; }

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Methods and Operators

        #endregion
    }
}
