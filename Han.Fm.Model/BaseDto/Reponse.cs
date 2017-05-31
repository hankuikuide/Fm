/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/19 14:19:41
 * ***********************************************/

namespace Han.Fm.Model.BaseDto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Response<T>
    {
        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public bool IsSuccess
        {
            get
            {
                return string.IsNullOrEmpty(this.ErrMsg);
            }
        }

        public T Result { get; set; }
    }

}
