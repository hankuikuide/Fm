using Han.Infrastructure;
using Han.Log;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Han.DbLight.Oracle
{
    public class DbTool
    {
        public static OracleDbType GetOracleDbType(object value)
        {
            var dataType = OracleDbType.NVarchar2;
            if (value is Enum)
            {
                dataType = OracleDbType.Char;
            }
            else if (value is string)
            {
                dataType = OracleDbType.Varchar2;
                if (value.ToString().Length > 2000)
                {
                    dataType = OracleDbType.Clob;
                }
            }
            else if (value is DateTime)
            {
                dataType = OracleDbType.TimeStamp;
            }
            else if (value is int || value is short)
            {
                dataType = OracleDbType.Int32;
            }
            else if (value is long)
            {
                dataType = OracleDbType.Int64;
            }
            else if (value is decimal || value is double || value is float)
            {
                dataType = OracleDbType.Decimal;
            }
            else if (value is Guid)
            {
                dataType = OracleDbType.Varchar2;
            }
            else if (value is bool)
            {
                dataType = OracleDbType.Byte;
            }
            else if (value is byte || value is byte[])
            {
                dataType = OracleDbType.Clob;
            }
            else if (value is char)
            {
                dataType = OracleDbType.Char;
            }

            return dataType;
        }

        public static OracleDbType GetOracleDbType(object[] values)
        {
            object value = new object();
            foreach (var o in values)
            {
                if (o != null)
                {
                    value = o;
                    break;
                }
            }
            return GetOracleDbType(value);
        }

        public static List<string> GetColumns(IDataRecord record)
        {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                //.ToUpper()可忽略大小写转换
                columnNames.Add(record.GetName(i).ToUpper());
            }
            return columnNames;
        }

        public static string ToDbString(object obj, Func<object, string> customConverter = null)
        {
            if (obj == null)
                return "NULL";
            Type type = obj.GetType();
            if (type == typeof(string))
            {
                string s = obj as string;
                return String.Format("'{0}'", s.Replace("'", "''"));
            }
            if (type == typeof(DateTime))
            {
                DateTime d = (DateTime)obj;
                return String.Format("to_date('{0}', 'dd/mm/yyyy hh24:mi')", d.ToString("dd/MM/yyyy HH:mm"));
            }
            if (type == typeof(bool))
            {
                bool v = (bool)obj;
                if (v) return "1";
                else return "0";
            }
            if (type == typeof(byte[]))
            {
                return Encoding.Default.GetString(obj as byte[]);
            }
            //
            if (type.IsEnum)
            {
                return Convert.ToInt32(obj).ToString();
            }
            return obj.ToString();
        }

        public static string GetLogSql(string sql, OracleParameterCollection parameters)
        {
            foreach (OracleParameter kv in parameters)
            {
                string regexKey = string.Format(@"{0}{1}(?=[\)\, ]?)", ":", kv.ParameterName);
                string part = ToDbString(kv.Value);
                sql = Regex.Replace(sql, regexKey, part);
            }

            return sql;
        }

        /// <summary>
        /// 处理空值 只处理string与日期。其他必须有调用方传入
        /// TODO 空值插入dbnull
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }
            if (type == typeof(DateTime?))
            {
                return DateTime.Now;
            }
            if (type == typeof(int?) || type == typeof(decimal?) || type == typeof(long?) || type == typeof(float?))
            {
                return 0;
            }

            throw new ArgumentException("批量操作遇到不能处理的空值");
        }

        /// <summary>
        /// 记录odp.net 执行的操作的第一行数据
        /// </summary>
        /// <param name="cmd"></param>
        public static void LogOperateDone(OracleCommand cmd)
        {
            if (cmd.ArrayBindCount > 0)
            {
                var sb = new StringBuilder(cmd.CommandText);
                for (int i = 0; i < cmd.Parameters.Count; i++)
                {
                    var objects = cmd.Parameters[i].Value;
                    var pName = cmd.Parameters[i].ParameterName;
                    if (objects != null)
                    {
                        if (objects.GetType().IsArray)
                        {
                            var tempArr = objects as object[];
                            sb.Replace(":" + pName + "", string.Format("'{0}'", tempArr[0].ToString()));
                        }
                        else
                        {
                            sb.Replace(":" + pName + "", string.Format("'{0}'", objects.ToString()));
                        }
                    }
                }

                Log.Logger.Log(Level.Debug, " 批量执行“" + ((object[])(cmd.Parameters[0].Value)).Count() + "”条，行①:  " + sb.ToString());
            }
        }
        /// <summary>
        /// 级别为Dug 时，记录批量处理的日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="cmd"></param>
        public static void WriteLog(Level logLevel, OracleCommand cmd)
        {
            if (logLevel == Level.Debug)
            {
                WriteLog(cmd);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="cmd"></param>
        public static void WriteLog(OracleCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {

                var builder = new StringBuilder();
                var rowsCount = cmd.ArrayBindCount;
                var columnsCount = cmd.Parameters.Count;
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var cmdText = cmd.CommandText;
                    for (var columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = cmd.Parameters[columnIndex].Value;
                        if (value != null)
                        {
                            if (value.GetType().IsArray)
                            {
                                var tempArr = value as object[];
                                cmdText = cmdText.Replace(string.Format(":{0}", cmd.Parameters[columnIndex].ParameterName), ConvertDbValue(cmd.Parameters[columnIndex].OracleDbTypeEx, tempArr[rowIndex]));
                            }
                            else
                            {
                                cmdText = cmdText.Replace(string.Format(":{0}", cmd.Parameters[columnIndex].ParameterName), ConvertDbValue(cmd.Parameters[columnIndex].OracleDbTypeEx, value));
                            }
                        }
                    }

                    builder.AppendFormat("{0};\r\n", cmdText);
                }
                Logger.Log(Level.Debug, " 批量执行“" + cmd.ArrayBindCount + "”条，行:\r\n Sql语句如下：\r\n" + builder);
            }
        }
        /// <summary>
        /// 转变常用的DbType
        /// </summary>
        /// <param name="oracleDbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertDbValue(OracleDbType oracleDbType, object value)
        {
            switch (oracleDbType)
            {
                case OracleDbType.Varchar2:
                case OracleDbType.NVarchar2:
                case OracleDbType.Char:
                case OracleDbType.NChar:
                    value = string.Format("'{0}'", value);
                    break;
                case OracleDbType.Date:
                case OracleDbType.TimeStamp:
                    value = string.Format("to_date('{0}','yyyy-MM-dd HH24:mi:ss') ", value);
                    break;
                case OracleDbType.Int16:
                case OracleDbType.Int32:
                case OracleDbType.Int64:
                case OracleDbType.Decimal:
                    break;
                default:
                    value = string.Format("'{0}'", value);
                    break;
            }

            return value.ToString();
        }

        /// <summary>
        /// 获取SQL数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domains"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static Dictionary<string, object[]> GetData<T>(IList<T> domains, Expression<Func<T, object>>[] cols) where T : class
        {
            Dictionary<string, object[]> columnRowData = new Dictionary<string, object[]>();
            foreach (var expression in cols)
            {
                object[] objs = domains.Select(expression.Compile()).ToArray();
                var type = expression.GetRightMostMember().Type;

                for (int j = 0; j < objs.Length; j++)
                {
                    if (objs[j] == null)
                    {
#if DEBUG
                        throw new ArgumentNullException(expression.PropertyName(), "批量更新包含空值");
#endif
                        //objs[j] = (object)DBNull.Value;
                        objs[j] = DbTool.GetDefaultValue(type);
                    }
                }
                var key = expression.PropertyName();
                if (!columnRowData.ContainsKey(key))
                {
                    columnRowData.Add(key, objs);
                }
            }

            return columnRowData;
        }

        public static object[] GetParamValues(object[] arry)
        {
            object value = new object();
            foreach (var o in arry)
            {
                if (o != null)
                {
                    value = o;
                    break;
                }
            }
            var type = value.GetType();

            if (type.IsEnum)
            {
                object[] result = new object[arry.Length];
                for (int i = 0; i < arry.Length; i++)
                {
                    // result[i] = (char)(int)arry[i];
                    result[i] = Convert.ToString((int)arry[i], 16);
                }
                return result;
            }
            else if (type == typeof(bool))
            {
                object[] result = new object[arry.Length];
                for (int i = 0; i < arry.Length; i++)
                {
                    bool v = (bool)arry[i];
                    if (v)
                        result[i] = "1";
                    else
                        result[i] = "0";
                }
                return result;
            }
            return arry;
        }

        public static object GetParamValue(object value)
        {
            var type = value.GetType();
            if (value == null)
            {
                return DBNull.Value;
            }
            else if (type.IsEnum)
            {
                return Convert.ToString((int)value, 16);
            }
            else if (type == typeof(bool))
            {
                bool v = (bool)value;
                if (v) { return "1"; }
                else { return "0"; }
            }
            else
            {
                return value;
            }
        }
    }
}
