/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/25 16:15:13
 * ***********************************************/

namespace Han.DbLight.MySQl
{
    using Log;
    using MySql.Data.MySqlClient;
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
    public class BatchMySqlHelper
    {
        private readonly DbProviderFactory _database;
        private MySqlQuerySession querySession;

        public BatchMySqlHelper(IQuerySession querySession)
        {
            this.querySession = (MySqlQuerySession)querySession;
            this._database = querySession.DbProviderFactory;
        }

        public DbProviderFactory MySqlDatabase
        {
            get { return _database; }
        }

        private MySqlParameter GetMySqlParamter(KeyValuePair<int, object[]> pair)
        {
            var dbType = GetMySqlDbType(pair.Value);
            MySqlParameter parameter = new MySqlParameter(string.Format("{0}", pair.Key), dbType);

            parameter.Direction = ParameterDirection.Input;
            parameter.MySqlDbType = dbType;
            //parameter.Value = GetBatchParamValues(pair.Value);
            parameter.Value = GetBatchParamValues(pair.Value)?.First();

            return parameter;
        }
        
        private object[] GetBatchParamValues(object[] array)
        {
            //TODO 这个方法要重构一下
            object value = new object();
            foreach (var o in array)
            {
                if (o != null)
                {
                    value = 0;
                }
            }

            var type = value.GetType();
            if (type.IsEnum)
            {
                object[] result = new object[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    result[i] = Convert.ToString((int)array[i], 16);
                }
                return result;
            }
            else if (type == typeof(bool))
            {
                object[] result = new object[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    result[i] = (bool)array[i] ? "i" : "0";
                }
                return result;
            }

            return array;

        }

        public int BatchInsertOrUpdate(string insertOrUpdateSql, Dictionary<int, object[]> columnRowData)
        {
            var iResult = 0;
            if (columnRowData.Count>0)
            {
                using (var cmd = this.GetMySqlCommand(insertOrUpdateSql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = insertOrUpdateSql;
                    cmd.CommandTimeout = 600;

                    foreach (var r in columnRowData)
                    {
                        cmd.Parameters.Add(this.GetMySqlParamter(r));
                    }
                    if (Transaction.Current != null)
                    {
                        cmd.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection as MySqlConnection;
                        cmd.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction as MySqlTransaction;
                        iResult = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using (var conn = querySession.OpenConnection())
                        {
                            cmd.Connection = conn;

                            iResult = cmd.ExecuteNonQuery();
                        }
                    }

                    WriteLog(Level.Debug, cmd);


                    return iResult;
                }
            }


            return iResult;
        }

        private void WriteLog(Level logLevel, MySqlCommand cmd)
        {
            if (logLevel == Level.Debug)
            {
                WriteLog(cmd);
            }
        }

        private void WriteLog(MySqlCommand cmd)
        {
            if (cmd.Parameters!=null && cmd.Parameters.Count>0)
            {
                var builder = new StringBuilder();
                var rowCount = 1;
                //var rowCount = (cmd.Parameters[0].Value as object[]).Length;
                var columnCount = cmd.Parameters.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    var cmdText = cmd.CommandText;
                    for (int j = 0; j < columnCount; j++)
                    {
                        var value = cmd.Parameters[j].Value;
                        if (value != null)
                        {
                            cmdText = cmdText.Replace(string.Format("?{0}",j), ConvertDbValue(cmd.Parameters[j].MySqlDbType,value));
                        }
                    }
                    builder.AppendFormat("{0};\r\n",cmdText);
                }
                Logger.Log(Level.Debug, "批量执行:" + rowCount + "条, SQL语句如下:/r/n" + builder);
            }
        }

        private string ConvertDbValue(MySqlDbType mySqlDbType, object value)
        {
            switch (mySqlDbType)
            {
                case MySqlDbType.Decimal:
                    break;
                case MySqlDbType.Byte:
                    break;
                case MySqlDbType.Int16:
                    break;
                case MySqlDbType.Int24:
                    break;
                case MySqlDbType.Int32:
                    break;
                case MySqlDbType.Int64:
                    break;
                case MySqlDbType.Float:
                    break;
                case MySqlDbType.Double:
                    break;
                case MySqlDbType.Timestamp:
                    break;
                case MySqlDbType.Date:
                    break;
                case MySqlDbType.Time:
                    break;
                case MySqlDbType.DateTime:
                    break;
                case MySqlDbType.Year:
                    break;
                case MySqlDbType.Newdate:
                    break;
                case MySqlDbType.VarString:
                    break;
                case MySqlDbType.Bit:
                    break;
                case MySqlDbType.JSON:
                    break;
                case MySqlDbType.NewDecimal:
                    break;
                case MySqlDbType.Enum:
                    break;
                case MySqlDbType.Set:
                    break;
                case MySqlDbType.TinyBlob:
                    break;
                case MySqlDbType.MediumBlob:
                    break;
                case MySqlDbType.LongBlob:
                    break;
                case MySqlDbType.Blob:
                    break;
                case MySqlDbType.VarChar:
                    break;
                case MySqlDbType.String:
                    break;
                case MySqlDbType.Geometry:
                    break;
                case MySqlDbType.UByte:
                    break;
                case MySqlDbType.UInt16:
                    break;
                case MySqlDbType.UInt24:
                    break;
                case MySqlDbType.UInt32:
                    break;
                case MySqlDbType.UInt64:
                    break;
                case MySqlDbType.Binary:
                    break;
                case MySqlDbType.VarBinary:
                    break;
                case MySqlDbType.TinyText:
                    break;
                case MySqlDbType.MediumText:
                    break;
                case MySqlDbType.LongText:
                    break;
                case MySqlDbType.Text:
                    break;
                case MySqlDbType.Guid:
                    break;
                default:
                    break;
            }

            return value.ToString();
        }

        private static MySqlDbType GetMySqlDbType(object[] values)
        {
            object value = new object();
            foreach (var v in values)
            {
                if (v != null)
                {
                    value = v;
                    break;
                }
            }
            if (value is string)
            {
                int temp = value.ToString().Length;
                foreach (var item in values)
                {
                    if (item.ToString().Length > temp)
                    {
                        value = item;
                        temp = value.ToString().Length;
                    }
                }
            }

            return GetMySqlDbType(value);
        }

        private static MySqlDbType GetMySqlDbType(object value)
        {
            MySqlDbType dataType = MySqlDbType.VarChar;
            Type type = value.GetType();

            if (type.IsEnum)
            {
                return MySqlDbType.VarChar;
            }
            if (value is string)
            {
                dataType = MySqlDbType.VarChar;
                if (value.ToString().Length > 2000)
                {
                    dataType = MySqlDbType.Blob;
                }
            }
            else if (value is DateTime)
            {
                dataType = MySqlDbType.DateTime;
            }
            else if (value is int || value is short)
            {
                dataType = MySqlDbType.Int32;
            }
            else if (value is long)
            {
                dataType = MySqlDbType.Int64;
            }
            else if (value is decimal || value is double || value is float)
            {
                dataType = MySqlDbType.Decimal;
            }
            else if (value is Guid)
            {
                dataType = MySqlDbType.Guid;
            }
            else if (value is bool)
            {
                dataType = MySqlDbType.Byte;
            }
            else if (value is byte || value is byte[])
            {
                dataType = MySqlDbType.Blob;
            }
            else if (value is char)
            {
                dataType = MySqlDbType.VarChar;
            }

            return dataType;
        }

        private MySqlCommand GetMySqlCommand(string sql)
        {
            var mySqlCommand = new MySqlConnection(querySession.ConnectoinString).CreateCommand();

            mySqlCommand.CommandText = sql;
            //mySqlCommand.BindByName = true;

            return mySqlCommand;
        }
    }
}
