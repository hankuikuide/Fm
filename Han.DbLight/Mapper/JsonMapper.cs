using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Han.Log;
using Han.DbLight.Extensions;

namespace Han.DbLight.Mapper
{
    public class JsonMapper : IRowMapper<StringBuilder>
    {

        /// <summary>
        /// 需要映射的属性名称
        /// </summary>
        protected List<string> ObjProperties = new List<string>();
        public StringBuilder MapRow(IDataRecord row)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            
           
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                foreach (var col in row.GetColumns())
                {
                    object value;
                    if (DbHelper.TryGetColumnValue(row, col, out value))
                    {
                        writer.WritePropertyName(col);
                        writer.WriteValue(value);

                    }

                }
               
                writer.WriteEndObject();

            }
            return sb;

        }

        public TablesInfo TablesInfo { get; set; }
    }
}
