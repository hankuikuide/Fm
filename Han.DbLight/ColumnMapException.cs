
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 映射异常
    /// </summary>
    public class ColumnMapException:Exception
    {
        private string col;

        public ColumnMapException(string colName):base(colName)
        {
            // TODO: Complete member initialization
            this.col = colName;
        }
        
       // public ColumnMapException(string column)
    }
}
