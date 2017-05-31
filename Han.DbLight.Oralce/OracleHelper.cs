using System;
using System.Collections.Generic;
using Han.Infrastructure;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Han.Log;
using Han.DbLight;

namespace Han.DbLight.Oracle
{
    /// <summary>
    /// 数据库操作类
    /// 参照微软DbHelper  并修改为单例模式实现  简化数据库支持：Oracle
    /// 作者：MJL  2015-03-25
    /// </summary>
    public class OracleHelper
    {
        private LogHelper log = new LogHelper(typeof(OracleHelper));
        private static object obj = new object();
        private static OracleHelper instance = null;
        [ThreadStatic]
        private OracleConnection conn;
        public static OracleHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new OracleHelper();
                        }
                    }
                }
                return new OracleHelper();
            }
        }

        public OracleConnection CreateConnection(string connectionString = null)
        {
            if (conn == null)
            {
                string connStr = "";
                // string dbKey = Global.CurrentRequest.DbKey;
                string dbKey = "";
                var dbConnections = ConfigHelper.Instance.GetSection("DBConnectionString");
                if (string.IsNullOrEmpty(dbKey))
                {
                    if (dbConnections.Count > 0)
                    {
                        connStr = dbConnections.First().Value;
                    }
                    else
                    {
                        connStr = ConfigHelper.Instance.Get("shenHeDb");
                    }
                }
                else
                {
                    connStr = dbConnections[dbKey];
                }

                if (string.IsNullOrEmpty(connStr))
                {
                    connStr = ConfigHelper.Instance.Get("drgsDb");
                }
                string oldStr = connectionString ?? connStr;
                string trueConn = new Infrastructure.SymmetricMethod().Decrypto(oldStr);
                conn = new OracleConnection(string.IsNullOrEmpty(trueConn) ? oldStr : trueConn);
            }
            //return new OracleConnection(connectionString ?? ConfigurationManager.AppSettings["DefaultConn"]);
            return conn;
        }

        public OracleCommand GetStoredProcCommond(string storedProcedure, string connectionString = null)
        {
            OracleCommand cmd = CreateConnection(connectionString).CreateCommand();
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;
            return cmd;
        }

        public OracleCommand GetSqlStringCommond(string sqlQuery, string connectionString = null)
        {
            OracleCommand cmd = CreateConnection(connectionString).CreateCommand();
            if (null != Transaction.Current)
            {
                cmd = (Transaction.Current.DbTransactionWrapper.DbTransaction.Connection as OracleConnection).CreateCommand();
            }
            else
            {
                cmd = CreateConnection(connectionString).CreateCommand();
            }
            cmd.CommandText = sqlQuery;
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;
            //if (null != Transaction.Current)
            //{
            //    cmd.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection as OracleConnection;
            //    cmd.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction as OracleTransaction;
            //}
            return cmd;
        }

        #region 增加参数

        public void AddParameterCollection(OracleCommand cmd, IEnumerable<OracleParameter> paramCollection)
        {
            foreach (OracleParameter param in paramCollection)
            {
                cmd.Parameters.Add(param);
            }
        }

        public void AddInParameters(OracleCommand cmd, IDictionary<string, object> dicParams)
        {
            foreach (var param in dicParams)
            {
                this.AddInParameter(cmd, param.Key, param.Value);
            }
        }

        public void AddBatchInParameters<T>(OracleCommand cmd, IList<T> domains, params Expression<Func<T, object>>[] cols) where T : class
        {
            Dictionary<string, object[]> columnRowData = DbTool.GetData(domains, cols);

            if (columnRowData.Count > 0)
            {
                string[] dbColumns = columnRowData.Keys.ToArray();

                // 绑定批处理的行数
                cmd.ArrayBindCount = columnRowData.Values.First().Length; // 很重要
                cmd.CommandTimeout = 600; // 10分钟

                foreach (string colName in dbColumns)
                {
                    this.AddBatchInParameter(cmd, colName, columnRowData[colName]);
                }
            }
        }

        public void AddBatchInParameters(OracleCommand cmd, Dictionary<string, object[]> columnRowData)
        {
            if (columnRowData.Count > 0)
            {
                string[] dbColumns = columnRowData.Keys.ToArray();

                // 绑定批处理的行数
                cmd.ArrayBindCount = columnRowData.Values.First().Length; // 很重要
                cmd.CommandTimeout = 600; // 10分钟

                foreach (string colName in dbColumns)
                {
                    this.AddBatchInParameter(cmd, colName, columnRowData[colName]);
                }
            }
        }

        public void AddOutParameter(OracleCommand cmd, string parameterName)
        {
            OracleParameter dbParameter = cmd.CreateParameter();
            dbParameter.OracleDbType = OracleDbType.NVarchar2;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = 4096;
            dbParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddOutParameter(OracleCommand cmd, string parameterName, OracleDbType dbType, int size)
        {
            OracleParameter dbParameter = cmd.CreateParameter();
            dbParameter.OracleDbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = size;
            dbParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddInParameter(OracleCommand cmd, string parameterName, object value)
        {
            OracleParameter dbParameter = new OracleParameter();
            dbParameter.ParameterName = parameterName;
            //dbParameter.OracleDbType = DbTool.GetOracleDbType(value);
            dbParameter.OracleDbTypeEx = DbTool.GetOracleDbType(value);
            dbParameter.Value = DbTool.GetParamValue(value);
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddBatchInParameter(OracleCommand cmd, string parameterName, object[] value)
        {
            if (value.Length > 0)
            {
                // 绑定批处理的行数
                cmd.ArrayBindCount = value.Length; // 很重要
                cmd.CommandTimeout = 600; // 10分钟
                OracleParameter dbParameter = new OracleParameter();
                dbParameter.ParameterName = parameterName;
                //dbParameter.OracleDbType = DbTool.GetOracleDbType(value);
                dbParameter.OracleDbTypeEx = DbTool.GetOracleDbType(value);
                dbParameter.Value = DbTool.GetParamValue(value);
                dbParameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(dbParameter);
            }
        }

        public void AddBatchInParameter(OracleCommand cmd, string parameterName, object value, int paramLength = 0)
        {
            var arrLen = cmd.ArrayBindCount;
            if (paramLength > 0)
            {
                arrLen = paramLength;
            }
            var arrValue = new object[arrLen];
            for (int i = 0; i < arrLen; i++)
            {
                arrValue[i] = value;
            }
            AddBatchInParameter(cmd, parameterName, arrValue);
        }

        public void AddInOutParameter(OracleCommand cmd, string parameterName, object value)
        {
            OracleParameter dbParameter = cmd.CreateParameter();
            dbParameter.OracleDbType = DbTool.GetOracleDbType(value);
            dbParameter.ParameterName = parameterName;
            dbParameter.Value = DbTool.GetParamValue(value);
            dbParameter.Direction = ParameterDirection.InputOutput;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddReturnParameter(OracleCommand cmd, string parameterName, OracleDbType dbType)
        {
            OracleParameter dbParameter = cmd.CreateParameter();
            dbParameter.OracleDbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(dbParameter);
        }

        public OracleParameter GetParameter(OracleCommand cmd, string parameterName)
        {
            return cmd.Parameters[parameterName];
        }

        #endregion

        #region 执行

        public DataTable ExecuteDataTable(OracleCommand cmd)
        {
            DataTable dataTable = new DataTable();
            try
            {
                log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
                if (null == Transaction.Current)
                {
                    cmd.Connection.Open();
                }
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dataTable);
            }
            catch (System.Exception ex)
            {
                dataTable = null;
                log.Error("ExecuteDataTable", ex);
                throw ex;
            }
            finally
            {
                if (null == Transaction.Current && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
            return dataTable;
        }

        public Task<DataTable> ExecuteDataTableAsync(OracleCommand cmd)
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<DataTable>((object tran) =>
            {
                DataTable rel = null;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = ExecuteDataTable(cmd);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        public int ExecuteNonQuery(OracleCommand cmd)
        {
            int ret = -1;
            try
            {
                if (cmd.ArrayBindCount > 0)
                {
#if DEBUG
                    DbTool.LogOperateDone(cmd);
#endif
                    DbTool.WriteLog(Level.Debug, cmd);
                }
                else
                {
                    log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
                }

                //执行批处理
                if (null == Transaction.Current)
                {
                    cmd.Connection.Open();
                }
                ret = cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                log.Error("ExecuteNonQuery", ex);
                throw ex;
            }
            finally
            {
                if (null == Transaction.Current && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
            return ret;
        }

        public Task<int> ExecuteNonQueryAsync(OracleCommand cmd)
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<int>((object tran) =>
            {
                int rel = -1;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = ExecuteNonQuery(cmd);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        public object ExecuteScalar(OracleCommand cmd)
        {
            object ret = null;
            try
            {
                log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
                if (null == Transaction.Current)
                {
                    cmd.Connection.Open();
                }
                ret = cmd.ExecuteScalar();
            }
            catch (System.Exception ex)
            {
                ret = null;
                log.Error("ExecuteScalar", ex);
                throw ex;
            }
            finally
            {
                if (null == Transaction.Current && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
            return ret;
        }

        public Task<object> ExecuteScalarAsync(OracleCommand cmd)
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<object>((object tran) =>
            {
                object rel = null;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = ExecuteScalar(cmd);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 谨慎使用 Reader记得关闭
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private OracleDataReader ExecuteReader(OracleCommand cmd)
        {
            log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
            if (null == Transaction.Current)
            {
                cmd.Connection.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// 批量更新 自带事物处理
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="insertOrUpdateSql">SQL执行模板</param>
        /// <param name="domains">实体</param>
        /// <param name="cols">需要更新的列</param>
        /// <returns></returns>
        public int BatchExecute<T>(string insertOrUpdateSql, IList<T> domains, params Expression<Func<T, object>>[] cols) where T : class
        {
            var cmd = GetSqlStringCommond(insertOrUpdateSql);
            this.AddBatchInParameters<T>(cmd, domains, cols);
            return this.ExecuteNonQuery(cmd);
        }

        public Task<int> BatchExecuteAsync<T>(string insertOrUpdateSql, IList<T> domains, params Expression<Func<T, object>>[] cols) where T : class
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<int>((object tran) =>
            {
                int rel = -1;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = BatchExecute<T>(insertOrUpdateSql, domains, cols);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 批量更新 自带事物处理
        /// </summary>
        /// <param name="insertOrUpdateSql">SQL执行模板</param>
        /// <param name="columnRowData">数据对象</param>
        /// <returns></returns>
        public int BatchExecute(string insertOrUpdateSql, Dictionary<string, object[]> columnRowData)
        {
            var iResult = 0;
            if (columnRowData.Count > 0)
            {
                var cmd = this.GetSqlStringCommond(insertOrUpdateSql);
                this.AddBatchInParameters(cmd, columnRowData);
                iResult = this.ExecuteNonQuery(cmd);
            }
            return iResult;
        }

        public Task<int> BatchExecuteAsync(string insertOrUpdateSql, Dictionary<string, object[]> columnRowData)
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<int>((object tran) =>
            {
                int rel = -1;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = BatchExecute(insertOrUpdateSql, columnRowData);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="domains">实体</param>
        /// <param name="cols">插入的列</param>
        /// <returns></returns>
        public int BatchInsert<T>(IList<T> domains, params Expression<Func<T, object>>[] cols) where T : class
        {
            List<string> usedProperies = cols.Select(expression => expression.PropertyName()).ToList();
            string sql = OracleSqlBuilder.InsertBuilder<T>(usedProperies);

            var cmd = this.GetSqlStringCommond(sql);
            this.AddBatchInParameters<T>(cmd, domains, cols);
            return this.ExecuteNonQuery(cmd);
        }

        public Task<int> BatchInsertAsync<T>(IList<T> domains, params Expression<Func<T, object>>[] cols) where T : class
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<int>((object tran) =>
            {
                int rel = -1;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = BatchInsert<T>(domains, cols);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="domains">实体</param>
        /// <param name="where">更新条件</param>
        /// <param name="cols">涉及的列</param>
        /// <returns></returns>
        public int BatchUpdate<T>(IList<T> domains, string where, params Expression<Func<T, object>>[] cols) where T : class
        {
            List<string> usedProperies = cols.Select(expression => expression.PropertyName()).ToList();
            string sql = OracleSqlBuilder.UpateBuilder<T>(where, usedProperies);

            var cmd = this.GetSqlStringCommond(sql);
            this.AddBatchInParameters<T>(cmd, domains, cols);
            return this.ExecuteNonQuery(cmd);
        }

        public Task<int> BatchUpdateAsync<T>(IList<T> domains, string where, params Expression<Func<T, object>>[] cols) where T : class
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<int>((object tran) =>
            {
                int rel = -1;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = BatchUpdate<T>(domains, where, cols);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="domains">实体</param>
        /// <param name="cols">删除涉及的列</param>
        /// <returns></returns>
        public int BatchDelete<T>(IList<T> domains, string where, params Expression<Func<T, object>>[] cols) where T : class
        {
            List<string> usedProperies = cols.Select(expression => expression.PropertyName()).ToList();
            string sql = OracleSqlBuilder.DeleteBuilder<T>(where);

            var cmd = this.GetSqlStringCommond(sql);
            this.AddBatchInParameters<T>(cmd, domains, cols);
            return this.ExecuteNonQuery(cmd);
        }

        public Task<int> BatchDeleteAsync<T>(IList<T> domains, string where, params Expression<Func<T, object>>[] cols) where T : class
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<int>((object tran) =>
            {
                int rel = -1;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = BatchDelete<T>(domains, where, cols);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        public T ExecuteModel<T>(OracleCommand cmd) where T : class
        {
            T t = default(T);
            OracleDataReader reader = null;
            try
            {
                log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
                if (null == Transaction.Current)
                {
                    cmd.Connection.Open();
                }
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                 //TODO 观察一下是否有问题
                  //  t = Cis.BaseModel.DeserializeObject.CreateItem<T>(reader);
                }
            }
            catch (System.Exception ex)
            {
                log.Error("ExecuteModel", ex);
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (null == Transaction.Current && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
            return t;
        }

        public Task<T> ExecuteModelAsync<T>(OracleCommand cmd) where T : class
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<T>((object tran) =>
            {
                T rel = null;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = ExecuteModel<T>(cmd);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 转换字段可区分大小写
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(OracleCommand cmd) where T : class
        {
            //return Cis.BaseModel.DeserializeObject.ConvertTo<T>(ExecuteDataTable(cmd)) as List<T>;
            List<T> list = null;
            OracleDataReader reader = null;
            try
            {
                log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
                if (null == Transaction.Current)
                {
                    cmd.Connection.Open();
                    reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    reader = cmd.ExecuteReader();
                }
                var columns = DbTool.GetColumns(reader);
                list = new List<T>();
                while (reader.Read())
                {
                    //TODO ??
                   // list.Add(Cis.BaseModel.DeserializeObject.CreateItem<T>(reader, columns));
                }
            }
            catch (System.Exception ex)
            {
                log.Error("ExecuteList", ex);
                list = null;
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (null == Transaction.Current && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
            return list;
        }

        public Task<List<T>> ExecuteListAsync<T>(OracleCommand cmd) where T : class
        {
            DependentTransaction dependentTransaction = Transaction.Current.DependentClone();
            return new Task<List<T>>((object tran) =>
            {
                List<T> rel = null;
                Transaction.Current = tran as Transaction;
                try
                {
                    rel = ExecuteList<T>(cmd);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Transaction.Current = null;
                }
                return rel;
            }, dependentTransaction);
        }

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="values"></param>
        public void AddTempTable(IEnumerable<object> values, string colName, string tableName = "Global_Temp_Search")
        {
            string sql = string.Format("insert into {0}({1}) values(:{1})", tableName, colName);
            var cmd = this.GetSqlStringCommond(sql);
            this.AddBatchInParameter(cmd, colName, values.ToArray());
            this.ExecuteNonQuery(cmd);
        }

        #endregion

        //#region 执行事务
        ////public DataSet ExecuteDataSet(OracleCommand cmd, Trans t)
        ////{
        ////    cmd.Connection = t.DbConnection;
        ////    cmd.Transaction = t.DbTrans;
        ////    DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
        ////    DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
        ////    dbDataAdapter.SelectCommand = cmd;
        ////    DataSet ds = new DataSet();
        ////    dbDataAdapter.Fill(ds);
        ////    return ds;
        ////}

        ////public DataTable ExecuteDataTable(DbCommand cmd, Trans t)
        ////{
        ////    cmd.Connection = t.DbConnection;
        ////    cmd.Transaction = t.DbTrans;
        ////    DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
        ////    DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
        ////    dbDataAdapter.SelectCommand = cmd;
        ////    DataTable dataTable = new DataTable();
        ////    dbDataAdapter.Fill(dataTable);
        ////    return dataTable;
        ////}

        //public OracleDataReader ExecuteReader(OracleCommand cmd, Trans t)
        //{
        //    log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
        //    cmd.Connection.Close();
        //    cmd.Connection = t.DbConnection;
        //    cmd.Transaction = t.DbTrans;
        //    OracleDataReader reader = cmd.ExecuteReader();
        //    DataTable dt = new DataTable();
        //    return reader;
        //}
        //public int ExecuteNonQuery(OracleCommand cmd, Trans t)
        //{
        //    log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
        //    cmd.Connection.Close();
        //    cmd.Connection = t.DbConnection;
        //    cmd.Transaction = t.DbTrans;
        //    int ret = cmd.ExecuteNonQuery();
        //    return ret;
        //}

        //public object ExecuteScalar(OracleCommand cmd, Trans t)
        //{
        //    log.Debug(DbTool.GetLogSql(cmd.CommandText, cmd.Parameters));
        //    cmd.Connection.Close();
        //    cmd.Connection = t.DbConnection;
        //    cmd.Transaction = t.DbTrans;
        //    object ret = cmd.ExecuteScalar();
        //    return ret;
        //}
        //#endregion
    }
}
