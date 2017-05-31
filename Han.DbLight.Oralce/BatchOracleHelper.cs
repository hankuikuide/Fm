using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Han.Log;
using Han.DbLight.Mapper;
using Oracle.ManagedDataAccess.Client;

namespace Han.DbLight.Oracle
{
    public class BatchOracleHelper
    {
        private readonly DbProviderFactory _database;
        private OracleQuerySession qs;
        public BatchOracleHelper(IQuerySession querySession)
        {
            qs = (OracleQuerySession)querySession;
            _database = qs.DbProviderFactory;
        }

        public DbProviderFactory OracleDatabase
        {
            get { return _database; }
        }

        private OracleParameter GetOracleParameter2(KeyValuePair<int, object[]> pair)
        {
            var dbType = GetOracleDbType(pair.Value);
            OracleParameter parameter = new OracleParameter(string.Format("{0}", pair.Key), dbType);

            parameter.Direction = ParameterDirection.Input;
            parameter.OracleDbTypeEx = dbType;

            parameter.Value = GetBatchParamValues(pair.Value);

            return parameter;
        }
        private OracleParameter GetOracleParameter(KeyValuePair<int, object[]> pair)
        {
            OracleParameter parameter = new OracleParameter();
            parameter.ParameterName = string.Format("{0}", pair.Key);
            Type type = pair.Value[0].GetType();
            parameter.OracleDbTypeEx = GetOracleDbType(pair.Value);
            parameter.Value = GetValue(pair.Value, type);

            return parameter;
        }

        private object[] GetValue(object[] arry, Type type)
        {

            if (type.IsEnum)
            {
                object[] result = new object[arry.Length];
                for (int i = 0; i < arry.Length; i++)
                {
                    result[i] = (char)(int)arry[i];

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
        private object[] GetBatchParamValues(object[] arry)
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

        /// <summary>
        /// 批量插入或者批量更新大数据
        /// </summary>
        /// <param name="insertOrUpdateSql">插入或者更新的sql语句</param>
        /// <param name="columnRowData">插入或者更新的参数对应的value的值</param>
        /// <returns></returns>
        public int BatchInsertOrUpdate(string insertOrUpdateSql, Dictionary<int, object[]> columnRowData)
        {
            var iResult = 0;
            if (columnRowData.Count > 0)
            {
                //columnRowData = ConvertNullValues(columnRowData);
                using (var cmd = this.GetOracleCommand(insertOrUpdateSql))
                {
                    // 绑定批处理的行数
                    cmd.ArrayBindCount = columnRowData.Values.First().Length; // 很重要
                    cmd.BindByName = true;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = insertOrUpdateSql;
                    cmd.CommandTimeout = 600; // 10分钟

                    foreach (var objectse in columnRowData)
                    {
                        cmd.Parameters.Add(GetOracleParameter2(objectse));
                    }

                    // 执行批处理
                    if (null != Transaction.Current)
                    {
                        cmd.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection as OracleConnection;
                        cmd.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction as OracleTransaction;
                        iResult = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using (var conn = qs.OpenConnection())
                        {
                            cmd.Connection = conn;
                            iResult = cmd.ExecuteNonQuery();
                        }

                    }

#if DEBUG
					LogOperateDone(cmd);
#endif

                    WriteLog(Level.Debug, cmd);

                    return iResult;
                }
            }

            return iResult;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="columnRowData"></param>
        /// <returns></returns>
        public List<TResult> Query<TResult>(string sql, Dictionary<int, object[]> columnRowData, Mapper.IRowMapper<TResult> rowMapper)
        {
            // var iResult = 0;
            List<TResult> data = new List<TResult>();
            rowMapper.TablesInfo = qs.DatabaseInfo.TablesInfo;
            if (columnRowData.Count > 0)
            {
                //columnRowData = ConvertNullValues(columnRowData);
                using (var cmd = this.GetOracleCommand(sql))
                {
                    // 绑定批处理的行数
                    cmd.ArrayBindCount = columnRowData.Values.First().Length; // 很重要
                    cmd.BindByName = true;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 600; // 10分钟

                    foreach (var objectse in columnRowData)
                    {
                        cmd.Parameters.Add(GetOracleParameter2(objectse));
                    }

                    // 执行批处理
                    if (null != Transaction.Current)
                    {
                        cmd.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection as OracleConnection;
                        cmd.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction as OracleTransaction;
                        using (var reader = cmd.ExecuteReader())
                        {
                            //
                            reader.FetchSize = reader.RowSize * 2000;
                            if (reader.HasRows)
                            {
                                var r = reader.Cast<IDataRecord>();
                                foreach (var dataRecord in r)
                                {
                                    var t = rowMapper.MapRow(dataRecord);
                                    data.Add(t);
                                }

                            }
                        }
                    }
                    else
                    {
                        using (var conn = qs.OpenConnection())
                        {
                            cmd.Connection = conn;
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    var r = reader.Cast<IDataRecord>();
                                    foreach (var dataRecord in r)
                                    {
                                        var t = rowMapper.MapRow(dataRecord);
                                        data.Add(t);
                                    }
                                }
                            }
                        }
                    }
#if DEBUG
					LogOperateDone(cmd);
#endif
                    WriteLog(Level.Debug, cmd);

                    return data;
                }
            }

            return data;
        }
        /// <summary>
        /// 记录odp.net 执行的操作的第一行数据
        /// </summary>
        /// <param name="cmd"></param>
        private void LogOperateDone(OracleCommand cmd)
        {
            var sb = new StringBuilder(cmd.CommandText);
            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                var objects = cmd.Parameters[i].Value as object[];
                if (objects != null)
                    sb.Replace(":" + i.ToString() + "", string.Format("'{0}'", objects[0].ToString()));
            }

            Log.Logger.Log(Level.Debug, " 批量执行“" + ((object[])(cmd.Parameters[0].Value)).Count() + "”条，行①:  " + sb.ToString());
        }

        /// <summary>
        /// 级别为Dug 时，记录批量处理的日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="cmd"></param>
        private void WriteLog(Level logLevel, OracleCommand cmd)
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
        private void WriteLog(OracleCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                var builder = new StringBuilder();
                var rowsCount = (cmd.Parameters[0].Value as object[]).Length;
                var columnsCount = cmd.Parameters.Count;
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var cmdText = cmd.CommandText;
                    for (var columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = cmd.Parameters[columnIndex].Value as object[];
                        if (value != null)
                        {
                            cmdText = cmdText.Replace(string.Format(":{0}", columnIndex), ConvertDbValue(cmd.Parameters[columnIndex].OracleDbTypeEx, value[rowIndex]));
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
        private string ConvertDbValue(OracleDbType oracleDbType, object value)
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
        //   public OracleDataReader ExecuteReader(string commandText, IDictionary<int, object> dbParameters)
        //   {
        //       using (var cmd = this.GetOracleCommand(commandText))
        //       {
        //           cmd.BindByName = true;
        //           cmd.CommandText = commandText;
        //           cmd.CommandType = CommandType.Text;

        //           if (dbParameters != null && dbParameters.Keys.Count > 0)
        //           {
        //               foreach (var key in dbParameters.Keys)
        //               {
        //                   var dbType = GetOracleDbType(dbParameters[key]);
        //                   using (var oraParam = new OracleParameter(string.Format("{0}", key), dbType))
        //                   {
        //                       oraParam.Direction = ParameterDirection.Input;
        //                       oraParam.OracleDbTypeEx = dbType;

        //                       oraParam.Value = dbParameters[key];
        //                       cmd.Parameters.Add(oraParam);
        //                   }
        //               }
        //           }

        //           if (null != Transaction.Current)
        //           {
        //               cmd.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection as OracleConnection;
        //               return cmd.ExecuteReader();
        //           }
        //           else
        //           {
        //using (var conn = qs.OpenConnection())
        //{
        //	cmd.Connection = conn;
        //	return cmd.ExecuteReader();
        //}

        //           }
        //       }
        //   }

        /// <summary>
        /// 根据数据类型获取OracleDbType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static OracleDbType GetOracleDbType(object value)
        {
            OracleDbType dataType = OracleDbType.Varchar2;
            Type type = value.GetType();
            if (type.IsEnum)
            {
                return OracleDbType.Char;
            }

            if (value is string)
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

        private static OracleDbType GetOracleDbType(object[] values)
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
            return GetOracleDbType(value);
        }

        private OracleCommand GetOracleCommand(string sql)
        {
            var oraclCommand = new OracleConnection(qs.ConnectionString).CreateCommand();
            oraclCommand.CommandText = sql;
            oraclCommand.BindByName = true;

            return oraclCommand;
        }
    }
}
