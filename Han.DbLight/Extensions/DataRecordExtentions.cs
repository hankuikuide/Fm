
namespace Han.DbLight.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DataRecordExtentions
    {
        public static List<string> GetColumns(this IDataRecord record)
        {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                columnNames.Add(record.GetName(i));
            }
            return columnNames;
        }
    }
}
