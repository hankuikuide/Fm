/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/25 13:45:01
 * ***********************************************/

namespace Han.DbLight.MySQl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MySqlDbTypeConverter : IDbTypeConverter
    {
        public void AddParam(DbCommand cmd, object value)
        {
            var name = string.Format(":{0}", cmd.Parameters.Count);
            DbParameter p = this.GetDbParameter(cmd, name, value);
            cmd.Parameters.Add(p);
        }

        public void AddParams(DbCommand cmd, params object[] args)
        {
            foreach (var item in args)
            {
                AddParam(cmd, item);
            }
        }

        public void AddParams(DbCommand cmd, IDictionary<string, object> dbParameters)
        {
            foreach (var kv in dbParameters)
            {
                DbParameter parameter = this.GetDbParameter(cmd, kv.Key, kv.Value);
                cmd.Parameters.Add(parameter);
            }
        }

        public DbType ClrToDbType(Type type)
        {
            throw new NotImplementedException();
        }

        public Type DbTypeToClr(DbType dbType)
        {
            throw new NotImplementedException();
        }

        public DbParameter GetDbParameter(DbCommand cmd, string name, object value)
        {
            DbParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                var type = value.GetType();
                if (type.IsEnum)
                {
                    parameter.Value = Convert.ToString((int)value, 16);
                }
                else if (type == typeof(bool))
                {
                    parameter.Value = (bool)value ? "1" : "0";
                }
                else
                {
                    parameter.Value = value;
                }
            }

            return parameter;
        }

        public string ToDbString(object obj, Func<object, string> customConverter = null)
        {
            if (obj == null)
            {
                return "NULL";
            }

            Type type = obj.GetType();
            if (type == typeof(string))
            {
                string s = obj as string;
                return string.Format("'{0}'", s.Replace("'", "''"));
            }
            if (type == typeof(DateTime))
            {
                DateTime d = (DateTime)obj;
                return string.Format("to_date('{0}','dd/mm/yyyy hh24:mi')", d.ToString("dd/MM/yyyy HH:mm"));
            }
            if (type == typeof(bool))
            {
                bool v = (bool)obj;
                return v ? "1" : "0";
            }
            if (type.IsEnum)
            {
                return Convert.ToInt32(obj).ToString();
            }
            return obj.ToString();
        }

        public IDictionary<string, object> ToDicParams(params object[] args)
        {
            IDictionary<string, object> dbs = new Dictionary<string, object>();
            int i = 0;
            foreach (object item in args)
            {
                dbs.Add(i.ToString(), item);
                i++;
            }
            return dbs;
        }
    }
}
